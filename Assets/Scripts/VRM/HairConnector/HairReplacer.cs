using UnityEngine;
using System.Collections.Generic;
using UniVRM10;
using UniGLTF;
using System;


public class HairReplacer : MonoBehaviour
{
    public GameObject sourceModel; // Модель, из которой берутся волосы
    private GameObject targetModel; // Модель, в которую переносятся волосы

    public void ReplaceHair(GameObject gameObject)
    {
        targetModel = gameObject;

        if (sourceModel == null || targetModel == null)
        {
            Debug.LogError("Source or Target model is not set.");
            return;
        }

        SkinnedMeshRenderer sourceSkinnedMesh = CommonScripts.FindMeshRenderer(sourceModel, "Hair");
        SkinnedMeshRenderer targetSkinnedMesh = CommonScripts.FindMeshRenderer(targetModel, "Hair");

        // Найти J_Bip_C_Head в обеих моделях
        GameObject sourceHead = CommonScripts.FindChildByName(sourceModel, "J_Bip_C_Head");
        GameObject targetHead = CommonScripts.FindChildByName(targetModel, "J_Bip_C_Head");

        if (sourceHead == null || targetHead == null)
        {
            Debug.LogError("J_Bip_C_Head not found in one of the models.");
            return;
        }

        // Удаляем старые волосы из targetModel
        List<GameObject> targetHairObjects = CommonScripts.FindDirectChildrenBySubstring(targetHead, "_Hair");
        foreach (GameObject hair in targetHairObjects)
        {
            DestroyImmediate(hair);
        }

        targetModel.GetComponent<Vrm10Instance>().SpringBone.Springs.RemoveAll(spring => spring.Name == "Hair");

        // Переносим волосы из sourceModel в targetModel
        List<GameObject> sourceHairObjects = CommonScripts.FindDirectChildrenBySubstring(sourceHead, "_Hair");
        foreach (GameObject hair in sourceHairObjects)
        {
            GameObject newHair = Instantiate(hair, targetHead.transform);
            newHair.name = hair.name;
            HairSpringBoneReplace(newHair);
        }

        Mesh hairMesh = HairMeshReplace(sourceSkinnedMesh, targetSkinnedMesh);

        // Сохраняем меш как ассет
        // string path = "Assets/GeneratedMeshes/ScaledMesh_Hair.asset";
        // AssetDatabase.CreateAsset(hairMesh, path);
        // AssetDatabase.SaveAssets();

        Debug.Log("Hair replacement completed.");
    }

    Transform[] MapBones(Transform[] sourceBones, GameObject targetModel)
    {
        Transform[] mappedBones = new Transform[sourceBones.Length];
        foreach (var sourceBone in sourceBones)
        {
            Transform matchedBone = FindBoneInHierarchy(targetModel.transform, sourceBone.name);
            if (matchedBone == null)
            {
                Debug.LogError($"Bone {sourceBone.name} not found in target model.");
            }
            mappedBones[Array.IndexOf(sourceBones, sourceBone)] = matchedBone;
        }
        return mappedBones;
    }

    Transform FindBoneInHierarchy(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            Transform result = FindBoneInHierarchy(child, name);
            if (result != null) return result;
        }
        return null;
    }
    // Transform[] MapBones(Transform[] sourceBones, GameObject targetModel)
    // {
    //     Transform[] mappedBones = new Transform[sourceBones.Length];
    //     Transform[] targetBones = targetModel.GetComponentsInChildren<Transform>();

    //     for (int i = 0; i < sourceBones.Length; i++)
    //     {
    //         foreach (var bone in targetBones)
    //         {
    //             if (bone.name == sourceBones[i].name)
    //             {
    //                 mappedBones[i] = bone;
    //                 break;
    //             }
    //         }
    //     }
    //     return mappedBones;
    // }

    Mesh HairMeshReplace(SkinnedMeshRenderer sourceSkinnedMesh, SkinnedMeshRenderer targetSkinnedMesh)
    {
        Mesh hairMesh = Instantiate(sourceSkinnedMesh.sharedMesh);
        targetSkinnedMesh.sharedMesh = hairMesh;
        targetSkinnedMesh.sharedMaterials = sourceSkinnedMesh.sharedMaterials;
        targetSkinnedMesh.bones = MapBones(sourceSkinnedMesh.bones, targetModel);
        targetSkinnedMesh.sharedMesh.RecalculateBounds();
        return hairMesh;
    }

    void HairSpringBoneReplace(GameObject newHair)
    {
        // Получаем Vrm10Instance для исходной и целевой моделей
        var sourceVrmInstance = sourceModel.GetComponent<Vrm10Instance>();
        var targetVrmInstance = targetModel.GetComponent<Vrm10Instance>();

        if (sourceVrmInstance == null || targetVrmInstance == null)
        {
            Debug.LogError("Vrm10Instance not found on source or target model.");
            return;
        }

        var sourceSprings = sourceVrmInstance.SpringBone.Springs;
        var targetSprings = targetVrmInstance.SpringBone.Springs;

        // Создаем новый Spring
        var newSpring = new Vrm10InstanceSpringBone.Spring("Hair");
        newSpring.ColliderGroups = targetVrmInstance.SpringBone.ColliderGroups;

        // Используем AddJointRecursive для добавления Joints из newHair
        var rootJoint = newHair.GetComponent<VRM10SpringBoneJoint>();
        if (rootJoint == null)
        {
            rootJoint = newHair.AddComponent<VRM10SpringBoneJoint>();
            Debug.LogWarning("VRM10SpringBoneJoint component was missing on newHair and has been added.");
        }

        // Добавляем все Joints рекурсивно в новый Spring
        SpringBoneJointUtility springBoneJointUtility = new();
        springBoneJointUtility.AddJointRecursive(newHair.transform, rootJoint);

        // Сбор всех созданных Joints в новый Spring
        var jointList = new List<VRM10SpringBoneJoint>();
        springBoneJointUtility.GetJoints(newHair.transform, jointList);
        foreach (var joint in jointList)
        {
            newSpring.Joints.Add(joint);
        }

        // Добавляем новый Spring в целевой список
        targetSprings.Add(newSpring);

        Debug.Log($"New spring '{newSpring.Name}' added with {newSpring.Joints.Count} joints.");
    }
}


public class SpringBoneJointUtility
{
    public void AddJointRecursive(Transform target, VRM10SpringBoneJoint source)
    {
        // Проверяем, есть ли компонент VRM10SpringBoneJoint, если нет — добавляем.
        var joint = target.gameObject.GetOrAddComponent<VRM10SpringBoneJoint>();

        // Копируем свойства из источника
        joint.m_stiffnessForce = source.m_stiffnessForce;
        joint.m_gravityPower = source.m_gravityPower;
        joint.m_gravityDir = source.m_gravityDir;
        joint.m_dragForce = source.m_dragForce;
        joint.m_jointRadius = source.m_jointRadius;

        // Рекурсивно обрабатываем дочерние элементы
        for (int i = 0; i < target.childCount; i++)
        {
            AddJointRecursive(target.GetChild(i), source);
        }
    }

    public void GetJoints(Transform target, List<VRM10SpringBoneJoint> joints)
    {
        // Проверяем, есть ли на объекте компонент VRM10SpringBoneJoint
        if (target.TryGetComponent<VRM10SpringBoneJoint>(out var joint))
        {
            joints.Add(joint);
        }

        // Рекурсивно проверяем всех потомков
        for (int i = 0; i < target.childCount; i++)
        {
            GetJoints(target.GetChild(i), joints);
        }
    }
}




// public class HairSpringBoneUpdater : MonoBehaviour
// {
//     public GameObject sourceModel; // Исходная модель
//     private GameObject targetModel; // Целевая модель

//     public void UpdateSpringBones(GameObject gameObject)
//     {
//         targetModel = gameObject;

//         if (sourceModel == null || targetModel == null)
//         {
//             Debug.LogError("Source or Target model is not set.");
//             return;
//         }

//         // Получаем Vrm10Instance для обеих моделей
//         var sourceVrmInstance = sourceModel.GetComponent<Vrm10Instance>();
//         var targetVrmInstance = targetModel.GetComponent<Vrm10Instance>();

//         if (sourceVrmInstance == null || targetVrmInstance == null)
//         {
//             Debug.LogError("Vrm10Instance not found on one of the models.");
//             return;
//         }

//         // Копируем Springs из исходной модели
//         var sourceSprings = sourceVrmInstance.SpringBone.Springs;
//         var targetSprings = targetVrmInstance.SpringBone.Springs;

//         foreach (var spring in sourceSprings)
//         {
//             // // Создаем новый Spring для целевой модели
//             Vrm10InstanceSpringBone newSpring = new Vrm10InstanceSpringBone();
//             newSpring.Springs.Name = "1";
//             // newSpring.Springs.Add(spring);
//             // // Добавляем новый Spring в целевую модель
//             targetSprings.Add(spring);
//         }

//         Debug.Log("Spring bones updated successfully.");
//     }

//     private Transform FindBoneInTarget(string boneName, GameObject targetModel)
//     {
//         if (string.IsNullOrEmpty(boneName))
//         {
//             return null;
//         }

//         foreach (var bone in targetModel.GetComponentsInChildren<Transform>())
//         {
//             if (bone.name == boneName)
//             {
//                 return bone;
//             }
//         }

//         return null;
//     }
// }



// public class SpringBoneSetup
// {
//     public void CreateSpringBone(VRMC_springBone sourceSpringBone, GameObject targetModel)
//     {
//         // Создаём новый экземпляр VRMC_springBone
//         var newSpringBone = new VRMC_springBone();
//         // {
//         //     SpecVersion = sourceSpringBone.SpecVersion,
//         //     Colliders = new List<VRMC_springBone.Collider>(),
//         //     ColliderGroups = new List<VRMC_springBone.ColliderGroup>(),
//         //     Springs = new List<VRMC_springBone.Spring>()
//         // };

//         // Копируем Springs из sourceSpringBone
//         foreach (var sourceSpring in sourceSpringBone.Springs)
//         {
//             newSpringBone.ColliderGroups = new List<VRMC_springBone.ColliderGroup>(sourceSpring.ColliderGroups),
//             {
//                 Name = sourceSpring.Name,
//                 ColliderGroups = new List<VRMC_springBone.ColliderGroup>(sourceSpring.ColliderGroups),
//                 Joints = new List<VRMC_springBone.Spring.Joint>(),
//                 Center = FindBoneInTarget(sourceSpring.Center?.name, targetModel) // Ищем Transform в целевой модели
//             };

//             // Копируем Joints
//             foreach (var sourceJoint in sourceSpring.Joints)
//             {
//                 var targetJointTransform = FindBoneInTarget(sourceJoint.Transform?.name, targetModel);
//                 if (targetJointTransform != null)
//                 {
//                     var newJoint = new VRMC_springBone.Spring.Joint
//                     {
//                         Transform = targetJointTransform,
//                         Stiffness = sourceJoint.Stiffness,
//                         GravityPower = sourceJoint.GravityPower,
//                         GravityDir = sourceJoint.GravityDir,
//                         DragForce = sourceJoint.DragForce,
//                         HitRadius = sourceJoint.HitRadius
//                     };

//                     newSpring.Joints.Add(newJoint);
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"Joint {sourceJoint.Transform?.name} not found in target model.");
//                 }
//             }

//             newSpringBone.Springs.Add(newSpring);
//         }

//         Debug.Log("SpringBone setup complete.");
//     }

//     private Transform FindBoneInTarget(string boneName, GameObject targetModel)
//     {
//         if (string.IsNullOrEmpty(boneName)) return null;

//         foreach (var bone in targetModel.GetComponentsInChildren<Transform>())
//         {
//             if (bone.name == boneName)
//             {
//                 return bone;
//             }
//         }

//         return null;
//     }
// }



// public class HairSpringBoneUpdater : MonoBehaviour
// {
//     public GameObject sourceModel; // Исходная модель
//     private GameObject targetModel; // Целевая модель

//     public void UpdateSpringBones(GameObject gameObject)
//     {
//         targetModel = gameObject;

//         if (sourceModel == null || targetModel == null)
//         {
//             Debug.LogError("Source or Target model is not set.");
//             return;
//         }

//         // Получаем Vrm10Instance для обеих моделей
//         var sourceVrmInstance = sourceModel.GetComponent<Vrm10Instance>();
//         var targetVrmInstance = targetModel.GetComponent<Vrm10Instance>();
//         // var newSpring = new VRMC_springBone();

//         if (sourceVrmInstance == null || targetVrmInstance == null)
//         {
//             Debug.LogError("Vrm10Instance not found on one of the models.");
//             return;
//         }

//         // Копируем Springs из исходной модели
//         var sourceSprings = sourceVrmInstance.SpringBone.Springs;
//         var targetSprings = targetVrmInstance.SpringBone.Springs;

//         foreach (var spring in sourceSprings)
//         {
//             // Создаем новый Spring для целевой модели
//             var newSpring = new Vrm10InstanceSpringBone.Spring(spring.Name)
//             {
//                 Center = FindBoneInTarget(spring.Center?.name, targetModel),
//                 // ColliderGroups = spring.ColliderGroups.ToList(), // Копируем ColliderGroups
//                 Joints = new List<VRM10SpringBoneJoint>() // Новый список для Joints
//             };

//             foreach (var joint in spring.Joints)
//             {
//                 // Ищем соответствующую Transform в целевой модели
//                 var targetJointTransform = FindBoneInTarget(joint.transform.name, targetModel);
//                 if (targetJointTransform != null)
//                 {
//                     // Добавляем новый VRM10SpringBoneJoint на Transform
//                     var newJoint = targetJointTransform.gameObject.AddComponent<VRM10SpringBoneJoint>();

//                     // Копируем параметры из исходного Joint
//                     newJoint.m_stiffnessForce = joint.m_stiffnessForce;
//                     newJoint.m_gravityPower = joint.m_gravityPower;
//                     newJoint.m_gravityDir = joint.m_gravityDir;
//                     newJoint.m_dragForce = joint.m_dragForce;
//                     newJoint.m_jointRadius = joint.m_jointRadius;

//                     // Добавляем новый Joint в список Joints
//                     newSpring.Joints.Add(newJoint);
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"Joint {joint.transform.name} not found in target model.");
//                 }
//             }

//             // Добавляем новый Spring в целевую модель
//             targetSprings.Add(newSpring);
//         }

//         Debug.Log("Spring bones updated successfully.");
//     }

//     private Transform FindBoneInTarget(string boneName, GameObject targetModel)
//     {
//         if (string.IsNullOrEmpty(boneName))
//         {
//             return null;
//         }

//         foreach (var bone in targetModel.GetComponentsInChildren<Transform>())
//         {
//             if (bone.name == boneName)
//             {
//                 return bone;
//             }
//         }

//         return null;
//     }
// }


// public class HairSpringBoneUpdater : MonoBehaviour
// {
//     public GameObject sourceModel; // Исходная модель
//     private GameObject targetModel; // Целевая модель

//     public void UpdateSpringBones(GameObject gameObject)
//     {
//         targetModel = gameObject;

//         if (sourceModel == null || targetModel == null)
//         {
//             Debug.LogError("Source or Target model is not set.");
//             return;
//         }

//         // Получаем Vrm10Instance исходной и целевой моделей
//         var sourceVrmInstance = sourceModel.GetComponent<Vrm10Instance>();
//         var targetVrmInstance = targetModel.GetComponent<Vrm10Instance>();

//         if (sourceVrmInstance == null || targetVrmInstance == null)
//         {
//             Debug.LogError("Vrm10Instance not found on one of the models.");
//             return;
//         }

//         // Копируем Springs из исходной модели
//         var sourceSpringBones = sourceVrmInstance.SpringBone.Springs;
//         var targetSpringBones = targetVrmInstance.SpringBone.Springs;

//         foreach (var spring in sourceSpringBones)
//         {
//                         var newSpring = new VRM10SpringBone.Spring

//             // Создаем новую SpringBone для целевой модели
//             var newSpring = new VRM10SpringBone.Spring
//             {
//                 Name = spring.Name,
//                 Joints = new List<VRM10SpringBone.SpringJoint>()
//             };

//             foreach (var joint in spring.Joints)
//             {
//                 // Находим соответствующую кость в целевой модели
//                 var targetJointTransform = FindBoneInTarget(joint.Transform.name, targetModel);
//                 if (targetJointTransform != null)
//                 {
//                     var newJoint = new VRM10SpringBone.SpringJoint
//                     {
//                         Transform = targetJointTransform,
//                         Stiffness = joint.Stiffness,
//                         GravityPower = joint.GravityPower,
//                         GravityDir = joint.GravityDir,
//                         DragForce = joint.DragForce,
//                         HitRadius = joint.HitRadius
//                     };

//                     newSpring.Joints.Add(newJoint);
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"Joint {joint.Transform.name} not found in target model.");
//                 }
//             }

//             targetSpringBones.Add(newSpring);
//         }

//         Debug.Log("Spring bones updated successfully.");
//     }

//     private Transform FindBoneInTarget(string boneName, GameObject targetModel)
//     {
//         foreach (var bone in targetModel.GetComponentsInChildren<Transform>())
//         {
//             if (bone.name == boneName)
//             {
//                 return bone;
//             }
//         }

//         return null;
//     }
// }








// // Удаляем старые волосы из targetModel
//         List<GameObject> targetHairObjects = CommonScripts.FindDirectChildrenBySubstring(targetHead, "_Hair");
//         foreach (GameObject hair in targetHairObjects)
//         {
//             DestroyImmediate(hair);
//         }

//         // Переносим волосы из sourceModel в targetModel
//         List<GameObject> sourceHairObjects = CommonScripts.FindDirectChildrenBySubstring(sourceHead, "_Hair");
//         foreach (GameObject hair in sourceHairObjects)
//         {
//             GameObject newHair = Instantiate(hair, targetHead.transform);

//             // Переназначаем кости
//             SkinnedMeshRenderer sourceRenderer = hair.GetComponent<SkinnedMeshRenderer>();
//             SkinnedMeshRenderer targetRenderer = newHair.GetComponent<SkinnedMeshRenderer>();
//             if (sourceRenderer != null && targetRenderer != null)
//             {
//                 Transform[] targetBones = targetModel.GetComponentsInChildren<Transform>();
//                 Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

//                 foreach (Transform bone in targetBones)
//                 {
//                     boneMap[bone.name] = bone;
//                 }

//                 Transform[] newBones = new Transform[sourceRenderer.bones.Length];
//                 for (int i = 0; i < sourceRenderer.bones.Length; i++)
//                 {
//                     string boneName = sourceRenderer.bones[i].name;
//                     if (boneMap.TryGetValue(boneName, out Transform targetBone))
//                     {
//                         newBones[i] = targetBone;
//                     }
//                     else
//                     {
//                         Debug.LogWarning($"Bone {boneName} not found in target model.");
//                     }
//                 }

//                 targetRenderer.bones = newBones;
//                 targetRenderer.rootBone = boneMap.ContainsKey(sourceRenderer.rootBone.name) ? boneMap[sourceRenderer.rootBone.name] : null;
//             }
//         }
