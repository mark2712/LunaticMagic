using System;
using UnityEngine;

public class EyeFaceConnector
{
    private GameObject sourceModel;
    private GameObject targetModel;
    private FaceBodySearchPoints sourcePoints;
    private FaceBodySearchPoints targetPoints;
    private Vector3 scaleFactors;

    public EyeFaceConnector(GameObject sourceModel, GameObject targetModel, FaceBodySearchPoints sourcePoints, FaceBodySearchPoints targetPoints, Vector3 scaleFactors)
    {
        this.sourceModel = sourceModel;
        this.targetModel = targetModel;
        this.sourcePoints = sourcePoints;
        this.targetPoints = targetPoints;
        this.scaleFactors = scaleFactors;
    }

    /// <summary>
    /// Recursively searches for a GameObject in the hierarchy by name.
    /// </summary>
    /// <param name="parent">Parent GameObject to start the search from.</param>
    /// <param name="name">Name of the GameObject to find.</param>
    /// <returns>The found GameObject, or null if not found.</returns>
    private GameObject FindChildByName(GameObject parent, string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent.transform)
        {
            GameObject result = FindChildByName(child.gameObject, name);
            if (result != null)
                return result;
        }

        return null;
    }


    // public void AdjustEyePositions()
    // {
    //     GameObject sourceHead = FindChildByName(sourceModel, "J_Bip_C_Head");
    //     GameObject targetHead = FindChildByName(targetModel, "J_Bip_C_Head");

    //     GameObject sourceLeftEye = FindChildByName(sourceHead, "J_Adj_L_FaceEye");
    //     GameObject sourceRightEye = FindChildByName(sourceHead, "J_Adj_R_FaceEye");

    //     GameObject targetLeftEye = FindChildByName(targetHead, "J_Adj_L_FaceEye");
    //     GameObject targetRightEye = FindChildByName(targetHead, "J_Adj_R_FaceEye");

    //     if (sourceLeftEye == null || sourceRightEye == null || targetLeftEye == null || targetRightEye == null)
    //     {
    //         Debug.LogError("One or more eye bones are missing in the models.");
    //         return;
    //     }

    //     Transform sourceHeadTransform = sourceHead.transform;
    //     Transform targetHeadTransform = targetHead.transform;

    //     Transform sourceLeftEyeTransform = sourceLeftEye.transform;
    //     Transform sourceRightEyeTransform = sourceRightEye.transform;

    //     Transform targetLeftEyeTransform = targetLeftEye.transform;
    //     Transform targetRightEyeTransform = targetRightEye.transform;

    //     // Вычисляем центр головы (локальные координаты)
    //     Vector3 sourceHeadCenter = sourceHeadTransform.localPosition;
    //     Vector3 targetHeadCenter = targetHeadTransform.localPosition;

    //     // Разница в центре головы между моделями
    //     Vector3 headCenterOffset = sourceHeadCenter;

    //     // Вычисляем смещение глаз в локальных координатах исходной модели
    //     Vector3 leftEyeOffset = sourceLeftEyeTransform.localPosition - sourceHeadCenter;
    //     Vector3 rightEyeOffset = sourceRightEyeTransform.localPosition - sourceHeadCenter;

    //     // Преобразуем смещение в локальные координаты целевой модели
    //     Vector3 adjustedLeftEyePosition = targetHeadTransform.TransformPoint(leftEyeOffset + headCenterOffset);
    //     Vector3 adjustedRightEyePosition = targetHeadTransform.TransformPoint(rightEyeOffset + headCenterOffset);

    //     // Устанавливаем новые локальные позиции глаз
    //     targetLeftEyeTransform.localPosition = targetHeadTransform.InverseTransformPoint(adjustedLeftEyePosition);
    //     targetRightEyeTransform.localPosition = targetHeadTransform.InverseTransformPoint(adjustedRightEyePosition);

    //     Debug.Log("Eye positions have been adjusted successfully.");
    // }



    public void AdjustEyeWeightsWithOffset()
    {
        // Получаем `SkinnedMeshRenderer` для обеих моделей
        SkinnedMeshRenderer sourceRenderer = sourceModel.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer targetRenderer = targetModel.GetComponentInChildren<SkinnedMeshRenderer>();

        if (sourceRenderer == null || targetRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found in one of the models.");
            return;
        }

        // Находим головы и глаза
        GameObject sourceHead = FindChildByName(sourceModel, "J_Bip_C_Head");
        GameObject targetHead = FindChildByName(targetModel, "J_Bip_C_Head");

        GameObject sourceLeftEye = FindChildByName(sourceHead, "J_Adj_L_FaceEye");
        GameObject sourceRightEye = FindChildByName(sourceHead, "J_Adj_R_FaceEye");

        GameObject targetLeftEye = FindChildByName(targetHead, "J_Adj_L_FaceEye");
        GameObject targetRightEye = FindChildByName(targetHead, "J_Adj_R_FaceEye");

        if (sourceHead == null || targetHead == null || sourceLeftEye == null || sourceRightEye == null || targetLeftEye == null || targetRightEye == null)
        {
            Debug.LogError("Required bones (head/eyes) are missing in one of the models.");
            return;
        }

        // Вычисляем смещения для глаз
        Vector3 sourceLeftEyeLocalPos = sourceLeftEye.transform.localPosition;
        Vector3 sourceRightEyeLocalPos = sourceRightEye.transform.localPosition;
        Vector3 targetLeftEyeLocalPos = targetLeftEye.transform.localPosition;
        Vector3 targetRightEyeLocalPos = targetRightEye.transform.localPosition;

        Vector3 leftEyeOffset = sourceLeftEyeLocalPos - targetLeftEyeLocalPos;
        Vector3 rightEyeOffset = sourceRightEyeLocalPos - targetRightEyeLocalPos;

        // Получаем bindposes и кости для целевой модели
        Matrix4x4[] targetBindPoses = targetRenderer.sharedMesh.bindposes;
        Transform[] targetBones = targetRenderer.bones;

        int targetLeftEyeIndex = Array.IndexOf(targetBones, targetLeftEye.transform);
        int targetRightEyeIndex = Array.IndexOf(targetBones, targetRightEye.transform);

        if (targetLeftEyeIndex < 0 || targetRightEyeIndex < 0)
        {
            Debug.LogError("Eye bones not found in target model's SkinnedMeshRenderer.");
            return;
        }

        // Корректируем BoneWeight для глаз
        BoneWeight[] targetBoneWeights = targetRenderer.sharedMesh.boneWeights;
        Vector3[] vertices = targetRenderer.sharedMesh.vertices;

        for (int i = 0; i < targetBoneWeights.Length; i++)
        {
            BoneWeight weight = targetBoneWeights[i];
            Vector3 vertex = vertices[i];

            // Проверяем, относится ли вершина к левому или правому глазу
            if (weight.boneIndex0 == targetLeftEyeIndex || weight.boneIndex1 == targetLeftEyeIndex)
            {
                // Применяем смещение для левого глаза
                vertex += leftEyeOffset * weight.weight0;
            }
            else if (weight.boneIndex0 == targetRightEyeIndex || weight.boneIndex1 == targetRightEyeIndex)
            {
                // Применяем смещение для правого глаза
                vertex += rightEyeOffset * weight.weight0;
            }

            vertices[i] = vertex;
        }

        // Обновляем вершины и веса
        targetRenderer.sharedMesh.vertices = vertices;
        targetRenderer.sharedMesh.boneWeights = targetBoneWeights;

        Debug.Log("Eye weights adjusted successfully.");
    }
}
