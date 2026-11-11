using UnityEngine;
using UnityEditor;

public class FaceConnector : MonoBehaviour
{
    public GameObject sourceModel; // Модель, откуда берем часть
    private GameObject targetModel; // Модель, куда переносим часть
    private SkinnedMeshRenderer targetPart;
    private string partName = "Face";
    private Mesh scaledMesh;
    private FaceBodySearchPoints targetPoints;
    private FaceConnectorSearchTopPoint faceConnectorSearchTopPoint = new();
    private Vector3 targetTop1;

    public void CalcFace(GameObject gameObject)
    {
        targetModel = gameObject;

        targetPart = FindMeshRenderer(targetModel, partName);
        if (targetPart == null)
        {
            Debug.LogError($"Часть {partName} не найдена на целевой модели!");
            return;
        }

        targetTop1 = faceConnectorSearchTopPoint.VisualizeBoundary(targetPart);
        targetPoints = new(targetModel);
    }

    public void ReplaceFace()
    {
        SkinnedMeshRenderer sourcePart = FindMeshRenderer(sourceModel, partName);
        if (sourcePart == null)
        {
            Debug.LogError($"Часть {partName} не найдена на исходной модели!");
            return;
        }

        FaceBodySearchPoints sourcePoints = new(sourceModel);

        // Логика обработки коэффициентов масштабирования
        Vector3 scaleFactors = new Vector3(
            targetPoints.Xdistance / sourcePoints.Xdistance,
            targetPoints.Ydistance / sourcePoints.Ydistance,
            targetPoints.Ydistance / sourcePoints.Ydistance
        );

        // Масштабируем mesh
        scaledMesh = Instantiate(sourcePart.sharedMesh);
        ScaleMesh3(scaledMesh, scaleFactors);

        // Назначаем преобразованный меш targetPart
        targetPart.sharedMesh = scaledMesh;
        targetPart.sharedMaterials = sourcePart.sharedMaterials;
        targetPart.bones = MapBones(sourcePart.bones, targetModel);
        targetPart.sharedMesh.RecalculateBounds();

        // Вычисляем среднюю точку для исходной модели
        Vector3 sourceMiddle = (sourcePoints.top + sourcePoints.bottom) / 2;

        // Вычисляем среднюю точку для целевой модели
        Vector3 targetMiddle = (targetPoints.top + targetPoints.bottom) / 2;

        // Переводим точки в локальную систему координат
        Vector3 sourceMiddleLocal = sourcePart.transform.InverseTransformPoint(sourceMiddle);
        Vector3 targetMiddleLocal = targetPart.transform.InverseTransformPoint(targetMiddle);

        // Рассчитываем смещение
        Vector3 localOffset = targetMiddleLocal - sourceMiddleLocal;
        Vector3 worldOffset = targetPart.transform.TransformVector(localOffset);

        // Применяем смещение к позиции
        targetPart.transform.position = targetPoints.pos + worldOffset;

        Vector3 targetTop2 = faceConnectorSearchTopPoint.VisualizeBoundary(targetPart);

        // Применяем смещение
        CalculateOffset(scaledMesh, targetTop1 - targetTop2 - new Vector3(0, 0, targetPoints.Ydistance / 200));

        // исправляем зрачки
        EyeFaceConnector eyeFaceConnector = new(sourceModel, targetModel, sourcePoints, targetPoints, scaleFactors);
        eyeFaceConnector.AdjustEyeWeightsWithOffset();

        // // Сохраняем меш как ассет
        // string path = "Assets/GeneratedMeshes/ScaledMesh_" + targetPart.name + ".asset";
        // AssetDatabase.CreateAsset(scaledMesh, path);
        // AssetDatabase.SaveAssets();

        Debug.Log($"Часть {partName} успешно перенесена с масштабированием и центрированием!");
    }


    void ScaleMesh3(Mesh mesh, Vector3 scaleFactors)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3 center = mesh.bounds.center;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Смещение вершины относительно центра меша
            Vector3 offset = vertices[i] - center;

            // Масштабирование смещения
            offset.Scale(scaleFactors);

            // Возвращаем вершину к новой позиции относительно центра
            vertices[i] = center + offset;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    // Функция смещения Mesh
    void CalculateOffset(Mesh mesh, Vector3 offset)
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += offset;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    SkinnedMeshRenderer FindMeshRenderer(GameObject model, string name)
    {
        foreach (var renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (renderer.name == name)
                return renderer;
        }
        return null;
    }

    Transform[] MapBones(Transform[] sourceBones, GameObject targetModel)
    {
        Transform[] mappedBones = new Transform[sourceBones.Length];
        Transform[] targetBones = targetModel.GetComponentsInChildren<Transform>();

        for (int i = 0; i < sourceBones.Length; i++)
        {
            foreach (var bone in targetBones)
            {
                if (bone.name == sourceBones[i].name)
                {
                    mappedBones[i] = bone;
                    break;
                }
            }
        }
        return mappedBones;
    }
}
