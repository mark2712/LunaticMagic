using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayerController
{
    public static CameraPlayerController self;
    public Camera firstPersonCamera; // Камера от первого лица
    public Camera thirdPersonCamera; // Камера от третьего лица
    private CamerasController _camerasController;

    // private Transform playerCameraBody; // Тело игрока
    public Transform playerCameraBody;
    // private Transform playerModel;

    // public bool isPlayerMove = false;
    private Vector2 _lookInput;
    public float lookSpeed = 12f;

    public Transform _pointHeadCamera; // Точка, на которой фокусируется камера
    private Transform pointHeadAimCamera;
    private Transform pointHeadAimCamera_Position;

    [Header("Third-Person Camera Settings")]
    public Transform pointAimCamera;
    public float pointAimCameraX = 0.6f;
    public float pointAimCameraY = 0.9f;

    public float followDistance = 5f;
    public float followHeight = 2f;
    public float rotationSpeed = 20f; //вращение персонажа вокруг своей оси 
    public LayerMask cameraIgnoreLayerMask;

    // Минимальное и максимальное расстояние от камеры до точки головы
    public float minCameraDistance; // Минимальное расстояние
    public float maxCameraDistance; // Максимальное расстояние
    public float scrollSpeed = 2f; // Скорость изменения дистанции
    private float targetCameraDistanceNow; // Целевое расстояние камеры
    private float targetCameraDistanceFar;
    private float targetCameraDistanceNear;
    private bool targetCameraDistanceModeNear = true;
    private float currentSmoothVelocity; // Переменная для сглаживания движения
    private float currentCameraDistance;
    public float smoothCameraDistance = 0.07f; // Переменная для сглаживания движения
    public float cameraRadius = 0.1f; //радиус луча от головы к камере
    private Vector3 cameraBeforePosition = Vector3.zero;

    public CameraPlayerController(GameObject camera, CamerasController camerasController)
    {
        self = this;

        _camerasController = camerasController;
        playerCameraBody = camera.transform;
        cameraIgnoreLayerMask = LayerMask.GetMask("Player", "NPC");

        firstPersonCamera = playerCameraBody.Find("FirstPersonCamera").GetComponent<Camera>();
        thirdPersonCamera = playerCameraBody.Find("ThirdPersonCamera").GetComponent<Camera>();
        _camerasController.AddCamera(firstPersonCamera, CamerasController.Cameras.firstPersonCamera);
        _camerasController.AddCamera(thirdPersonCamera, CamerasController.Cameras.thirdPersonCamera);
        _camerasController.SetActiveCamera(CamerasController.Cameras.thirdPersonCamera);

        EditPointAimCameraXY();
    }
    public void SetLookInput(Vector2 lookInput) { _lookInput = lookInput; }
    public void Update() { SetLookInput(GameContext.InputController.Look.Value); UpdateCamera(); } // зависит от InputController
    public void Update(Vector2 lookInput) { SetLookInput(lookInput); UpdateCamera(); }
    public void UpdateCamera()
    {
        float mouseX = _lookInput.x * lookSpeed * Time.deltaTime;
        float mouseY = _lookInput.y * lookSpeed * Time.deltaTime;

        HandleThirdPersonRotation(mouseX, mouseY, thirdPersonCamera.transform);
        firstPersonCamera.transform.localRotation = thirdPersonCamera.transform.localRotation;

        cameraBeforePosition = thirdPersonCamera.transform.position;

        _lookInput = Vector2.zero;
    }

    public void OnScrollInputPerformed(InputAction.CallbackContext ctx)
    {
        targetCameraDistanceModeNear = true;
        float scrollValue = ctx.ReadValue<Vector2>().y;

        // Обновляем целевое расстояние камеры на основе текущего
        targetCameraDistanceNow = Mathf.Clamp(targetCameraDistanceFar - scrollValue * scrollSpeed, minCameraDistance, maxCameraDistance);

        targetCameraDistanceFar = targetCameraDistanceNow;
        // Debug.Log($"scrollValue {scrollValue} targetCameraDistanceNow {currentCameraDistance}");
    }

    private void HandleThirdPersonRotation(float mouseX, float mouseY, Transform cameraTransform)
    {
        // Quaternion oldPlayerModelRotation = playerModel.rotation;

        // Получаем текущую позицию камеры относительно персонажа
        Vector3 direction = cameraTransform.position - _pointHeadCamera.position;

        // Поворачиваем камеру вокруг игрока по горизонтали (Y-ось)
        Quaternion horizontalRotation = Quaternion.AngleAxis(mouseX, Vector3.up);
        // Поворачиваем камеру по вертикали (X-ось)
        Quaternion verticalRotation = Quaternion.AngleAxis(-mouseY, cameraTransform.right);

        direction = horizontalRotation * verticalRotation * direction;

        // Устанавливаем новую позицию камеры
        cameraTransform.position = _pointHeadCamera.position + direction;

        float eulerX = cameraTransform.eulerAngles.x;
        if (eulerX > 80f && eulerX < 180f)
        {
            // Камера слишком поднялась вверх, корректируем до 80°
            direction = Quaternion.AngleAxis(80f - eulerX, cameraTransform.right) * direction;
        }
        else if (eulerX > 180f && eulerX < 280f)
        {
            // Камера слишком опустилась вниз, корректируем до 280° (или -80°)
            direction = Quaternion.AngleAxis(280f - eulerX, cameraTransform.right) * direction;
        }

        // Устанавливаем новую позицию камеры, на этот раз с корректировкой вертикального угла
        cameraTransform.position = _pointHeadCamera.position + direction;

        RotatePlayerToCameraAndCompensate(cameraTransform, direction.normalized); // Компенсируем поворот камеры, чтобы она оставалась на месте при движении
        CameraRaycast(); // камера не проходит сквозь текстуры
        AdjustCameraDistance(smoothCameraDistance); // менять расстояние камеры колёсиком мыши (или если камера уперлась в потолок/стены и тд)
        PointAimCameraDynamicDistance();
        thirdPersonCamera.transform.LookAt(pointAimCamera);
    }

    private void RotatePlayerToCameraAndCompensate(Transform cameraTransform, Vector3 cameraDirection)
    {
        // Поворачиваем игрока в направлении камеры
        Vector3 lookDirection = cameraTransform.forward;
        lookDirection.y = 0; // Игнорируем вертикальную составляющую
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        // playerCameraBody.rotation = Quaternion.Slerp(playerCameraBody.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        playerCameraBody.rotation = targetRotation; // для мгновенного поворота

        // Компенсируем поворот камеры, чтобы она оставалась на месте
        Vector3 offset = cameraDirection * (cameraTransform.position - _pointHeadCamera.position).magnitude;
        cameraTransform.position = _pointHeadCamera.position + offset;
    }

    private void InitPointHeadPointCamera()
    {
        _pointHeadCamera = playerCameraBody.Find("PointHeadCamera");
        pointHeadAimCamera = playerCameraBody.Find("PointHeadAimCamera"); // контейнер точки прицела
        pointAimCamera = pointHeadAimCamera.Find("Point"); // текущая позиция точки прицела
        pointHeadAimCamera_Position = pointHeadAimCamera.Find("Position"); // максимальная позиция точки прицела

        _pointHeadCamera.localPosition = new Vector3(_pointHeadCamera.localPosition.x, pointAimCameraY, _pointHeadCamera.localPosition.z);
        pointHeadAimCamera_Position.localPosition = new Vector3(pointAimCameraX, _pointHeadCamera.localPosition.y, _pointHeadCamera.localPosition.z);
        pointAimCamera.position = pointHeadAimCamera_Position.position;
    }

    private void InitCameraDistanceParmsCalc()
    {
        float playerCameraBodySize = playerCameraBody.localScale.magnitude; // Учитываем общий размер
        cameraRadius = playerCameraBodySize * 0.22f;
        // minCameraDistance = Vector3.Distance(_pointHeadCamera.position, pointHeadAimCamera_Position.position) + cameraRadius;  // Минимальное расстояние
        minCameraDistance = cameraRadius * 1.1f;  // Минимальное расстояние
        maxCameraDistance = playerCameraBodySize * 3f; // Максимальное расстояние

        // Настраиваем scrollSpeed так, чтобы X прокруток покрывали весь диапазон
        scrollSpeed = (maxCameraDistance - minCameraDistance) / 15f;

        // Устанавливаем начальное расстояние камеры как среднее между min и max
        float midCameraDistance = (minCameraDistance + maxCameraDistance) / 2f;
        // float midCameraDistance = maxCameraDistance - minCameraDistance;
        targetCameraDistanceNow = midCameraDistance;
        targetCameraDistanceNear = midCameraDistance;
        targetCameraDistanceFar = midCameraDistance;
        currentCameraDistance = midCameraDistance;
    }

    private void AdjustCameraDistance(float smoothCamera)
    {
        // Плавное приближение текущего расстояния камеры к целевому
        currentCameraDistance = Mathf.SmoothDamp(
            currentCameraDistance,
            Mathf.Clamp(targetCameraDistanceNow, minCameraDistance, maxCameraDistance),
            ref currentSmoothVelocity,
            smoothCamera
        );

        // Рассчитываем направление от игрока до камеры
        Vector3 direction = (thirdPersonCamera.transform.position - _pointHeadCamera.position).normalized;
        // Обновляем позицию камеры
        thirdPersonCamera.transform.position = _pointHeadCamera.position + direction * currentCameraDistance;
    }


    private float nearModeExitTimer = 0f; // Таймер для выхода из режима ближнего расстояния
    private float nearModeDelay = 0.9f;    // Время задержки перед выходом из ближнего режима
    void CameraRaycast()
    {
        if (targetCameraDistanceModeNear)
        {
            if (CheckCameraPath(targetCameraDistanceFar)) // Сразу вычисляет новый targetCameraDistanceNear как побочный эффект
            {
                // На пути всё ещё что-то есть
                targetCameraDistanceNow = targetCameraDistanceNear;

                // Сбрасываем таймер, так как препятствие ещё существует
                nearModeExitTimer = 0f;
            }
            else
            {
                // Путь свободен, начинаем отсчёт таймера
                nearModeExitTimer += Time.deltaTime;

                if (nearModeExitTimer >= nearModeDelay)
                {
                    // Выходим из режима ближнего расстояния, если таймер истёк
                    targetCameraDistanceModeNear = false;
                    targetCameraDistanceNow = targetCameraDistanceFar;

                    // Сбрасываем таймер
                    nearModeExitTimer = 0f;
                }
            }
        }
        else
        {
            if (DidCameraPassThroughObstacle())
            {
                // Есть столкновение на пути или камера сейчас внутри чего-то
                targetCameraDistanceModeNear = true;

                // Сбрасываем таймер, так как переключились в ближний режим
                nearModeExitTimer = 0f;
            }
        }
    }

    private bool CheckCameraPath(float distance)
    {
        Vector3 pointHeadCamera = _pointHeadCamera.position;
        Vector3 directionToCamera = (thirdPersonCamera.transform.position - pointHeadCamera).normalized;
        Vector3 targetPointDistance2 = pointHeadCamera + directionToCamera * distance;

        RaycastHit[] hitsForward = Physics.SphereCastAll(pointHeadCamera, cameraRadius, directionToCamera, distance, ~cameraIgnoreLayerMask)
            .OrderBy(hit => hit.distance)
            .ToArray();

        RaycastHit[] hitsBackward = Physics.SphereCastAll(targetPointDistance2, cameraRadius, -directionToCamera, distance, ~cameraIgnoreLayerMask)
            .OrderBy(hit => hit.distance)
            .ToArray();

        // Проверяем столкновения
        if (hitsForward.Length > 0)
        {
            targetCameraDistanceNear = Mathf.Clamp(hitsForward[0].distance, minCameraDistance, maxCameraDistance);
            return true; // Есть столкновение
        }

        if (hitsBackward.Length > 0)
        {
            targetCameraDistanceNear = Mathf.Clamp(hitsBackward.Last().distance, minCameraDistance, maxCameraDistance);
            return true; // Есть столкновение
        }

        // Если препятствий не найдено
        targetCameraDistanceNow = targetCameraDistanceFar;
        return false;
    }

    private bool DidCameraPassThroughObstacle()
    {
        Vector3 cameraCurrentPosition = thirdPersonCamera.transform.position;

        // Кидаем луч от старой позиции к новой
        if (Physics.Raycast(cameraBeforePosition, (cameraCurrentPosition - cameraBeforePosition).normalized,
            out RaycastHit hit, Vector3.Distance(cameraBeforePosition, cameraCurrentPosition), ~cameraIgnoreLayerMask))
        {
            return true; // Камера прошла через препятствие
        }

        return false; // Нет столкновений
    }


    private void PointAimCameraDynamicDistance()
    {
        Vector3 headPosition = _pointHeadCamera.localPosition;
        Vector3 aimPosition = pointHeadAimCamera_Position.localPosition;

        // Приводим targetCameraDistanceNow в диапазон от 0 до 1
        float normalizedDistance = Mathf.InverseLerp(minCameraDistance, maxCameraDistance, targetCameraDistanceNow);
        // Нелинейное влияние: если расстояние близко к minCameraDistance, сильнее тянем к headPosition.x
        float influence = Mathf.Pow(1 - normalizedDistance, 6); // Чем меньше normalizedDistance, тем больше влияние
        // Вычисляем aimPositionX
        float aimPositionX = Mathf.Lerp(aimPosition.x, headPosition.x, influence);
        // Ограничиваем aimPositionX в диапазоне [headPosition.x, aimPosition.x]
        aimPositionX = Mathf.Clamp(aimPositionX, headPosition.x, aimPosition.x);
        // Вычисляем целевую позицию в локальных координатах
        Vector3 targetLocalPosition = new Vector3(aimPositionX, aimPosition.y, aimPosition.z);

        Vector3 targetGlobalPosition = pointHeadAimCamera_Position.parent.TransformPoint(targetLocalPosition);
        // Плавно перемещаем pointAimCamera к целевой позиции в глобальных координатах
        pointAimCamera.position = Vector3.Lerp(pointAimCamera.position, targetGlobalPosition, Time.deltaTime * 10f);
    }

    public void EditPointAimCameraXY()
    {
        InitPointHeadPointCamera();
        InitCameraDistanceParmsCalc();
        targetCameraDistanceModeNear = true;
    }
}