using System.Collections.Generic;
using UnityEngine;

public class CamerasController
{
    public static CamerasController self;
    public enum Cameras { firstPersonCamera, thirdPersonCamera }
    public List<Camera> camerasList; // Список камер
    public Camera activeCamera; // Активная камера
    public Cameras activeCameraName;

    public CamerasController()
    {
        self = this;
        camerasList = new List<Camera>();
    }

    public void AddCamera(Camera camera, Cameras cameraName)
    {
        // Проверяем, что камера не null
        if (camera == null)
        {
            Debug.LogError("Камера не может быть null!");
            return;
        }

        // Добавляем камеру в список по индексу, соответствующему enum
        int index = (int)cameraName;
        if (index >= camerasList.Count)
        {
            camerasList.AddRange(new Camera[index - camerasList.Count + 1]);
        }
        camerasList[index] = camera;
    }

    public void SetActiveCamera(Cameras cameraName)
    {
        // Получаем индекс камеры из enum
        int index = (int)cameraName;

        // Проверяем, существует ли камера с таким индексом
        if (index < 0 || index >= camerasList.Count || camerasList[index] == null)
        {
            Debug.LogError($"Камера с именем {cameraName} не найдена в списке!");
            return;
        }

        // Отключаем все камеры
        foreach (var cam in camerasList)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }
        }

        activeCameraName = cameraName;
        camerasList[index].gameObject.SetActive(true);
        activeCamera = camerasList[index];
    }

    public void ChangeActiveCameraThirdFirstPerson()
    {
        if (activeCameraName == Cameras.firstPersonCamera)
        {
            SetActiveCamera(Cameras.thirdPersonCamera);
        }
        else
        {
            SetActiveCamera(Cameras.firstPersonCamera);
        }
    }
}


// public class CamerasController
// {
//     public enum Cameras { firstPersonCamera, thirdPersonCamera }
//     public Camera[] camerasGameObjects;
//     public Camera activeCamera;

//     public CamerasController() { }

//     private void AddCamera(Camera camera, Cameras cameraName, bool activeNow = false)
//     {
//         camerasGameObjects add;
//         if (activeNow){
//             SetActiveCamera(camera);
//         }
//     }

//     private void SetActiveCamera(Cameras camera)
//     {
//         // Проверяем, существует ли камера в camerasGameObjects


//         // Отключить все камеры

//         // Включить выбранную камеру
//         camera.gameObject.SetActive(true);
//         activeCamera = camera;
//     }
// }

// public class CameraController : MonoBehaviour
// {
//     public Camera firstPersonCamera; // Камера от первого лица
//     public Camera thirdPersonCamera; // Камера от третьего лица
//     public Camera boundedFreeCamera; // Ограниченная свободная камера
//     public Camera unrestrictedFreeCamera; // Полностью свободная камера

//     private Camera _activeCamera; // Текущая активная камера
//     private bool _menuOpen = false; // Флаг для блокировки камеры при открытии меню

//     void Awake()
//     {
//         // Автоматически находим камеры по их именам
//         firstPersonCamera = transform.Find("FirstPersonCamera")?.GetComponent<Camera>();
//         thirdPersonCamera = transform.Find("ThirdPersonCamera")?.GetComponent<Camera>();
//         boundedFreeCamera = transform.Find("BoundedFreeCamera")?.GetComponent<Camera>();
//         unrestrictedFreeCamera = transform.Find("UnrestrictedFreeCamera")?.GetComponent<Camera>();

//         if (!firstPersonCamera || !thirdPersonCamera || !boundedFreeCamera || !unrestrictedFreeCamera)
//         {
//             Debug.LogError("Не удалось найти одну или несколько камер! Проверьте, что все камеры присутствуют и правильно названы.");
//         }

//         // Установить активной первую камеру
//         SetActiveCamera(thirdPersonCamera);
//         // SetActiveCamera(firstPersonCamera);
//     }

//     void Update()
//     {
//         if (_menuOpen) return; // Блокируем управление камерой при открытом меню

//         // Переключение между камерами
//         if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveCamera(firstPersonCamera);
//         if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveCamera(thirdPersonCamera);
//         if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveCamera(boundedFreeCamera);
//         if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveCamera(unrestrictedFreeCamera);
//     }

//     private void SetActiveCamera(Camera camera)
//     {
//         // Проверяем, существует ли камера
//         if (camera == null)
//         {
//             Debug.LogWarning("Камера не найдена! Пропускаем переключение.");
//             return;
//         }

//         // Отключить все камеры
//         if (firstPersonCamera) firstPersonCamera.gameObject.SetActive(false);
//         if (thirdPersonCamera) thirdPersonCamera.gameObject.SetActive(false);
//         if (boundedFreeCamera) boundedFreeCamera.gameObject.SetActive(false);
//         if (unrestrictedFreeCamera) unrestrictedFreeCamera.gameObject.SetActive(false);

//         // Включить выбранную камеру
//         camera.gameObject.SetActive(true);
//         _activeCamera = camera;
//     }

//     public Camera GetActiveCamera()
//     {
//         return _activeCamera;
//     }
// }
