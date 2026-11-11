using UnityEngine;
using System.Collections.Generic;
using static BonesData2;

public class AutoRagdollSetupVRM1 : MonoBehaviour
{
    public BonesData2 bonesData = new BonesData2();
    public GameObject mainObject;

    // [MenuItem("Tools/AutoRagdollSetupVRM1")]
    // public static void SetupRagdollMenu()
    // {
    //     GameObject selectedObject = Selection.activeGameObject;

    //     if (selectedObject == null)
    //     {
    //         Debug.LogError("Please select a GameObject with a humanoid model.");
    //         return;
    //     }

    //     AutoRagdollSetupVRM1 ragdollSetup = selectedObject.AddComponent<AutoRagdollSetupVRM1>();
    //     ragdollSetup.CreateRagdoll(selectedObject);
    // }

    public void CreateRagdoll(GameObject selectedObject)
    {
        mainObject = selectedObject;

        Dictionary<string, Transform> boneTransforms = new Dictionary<string, Transform>();

        // Добавляем Rigidbody и Joint для mainObject
        Rigidbody mainRigidbody = mainObject.AddComponent<Rigidbody>();
        mainRigidbody.mass = 20f;

        // Поиск всех костей
        foreach (var bone in bonesData.boneParameters)
        {
            Transform foundBone = FindBone(bone.name);
            if (foundBone != null)
            {
                boneTransforms[bone.name] = foundBone;

                Transform parentTransform =
                    (bone.parentName == null)
                        ? mainObject.transform
                        : FindBone(bone.parentName);

                if (parentTransform == null && bone.parentName != null)
                {
                    Debug.LogWarning($"Parent bone not found: {bone.parentName}");
                    continue;
                }

                AddRigidbodyAndJoint(foundBone, parentTransform, bone);
            }
            else
            {
                Debug.LogWarning($"Bone not found: {bone.name}");
            }
        }

        // Проверяем наличие Root и J_Bip_C_Hips
        if (!boneTransforms.TryGetValue("Root", out Transform rootTransform))
        {
            Debug.LogError("Root bone not found!");
            return;
        }

        if (!boneTransforms.TryGetValue("J_Bip_C_Hips", out Transform hipsTransform))
        {
            Debug.LogError("J_Bip_C_Hips bone not found!");
            return;
        }

        Debug.Log("Ragdoll setup complete for " + mainObject.name);

        // Включаем "Update When Offscreen" для всех SkinnedMeshRenderer
        SkinnedMeshRenderer[] skinnedMeshRenderers = mainObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            renderer.updateWhenOffscreen = true;
        }

        // Настройка позиций объектов
        // AdjustObjectPositions(mainObject, rootTransform, hipsTransform);

        // Установка коллайдеров
        // сreateCollidersVRM1.CreateAllPoints(mainObject);
    }

    /// <summary>
    /// Смещает координаты объектов, кроме Root и его содержимого, с учётом J_Bip_C_Hips.
    /// </summary>
    /// <param name="mainObject">Главный объект.</param>
    /// <param name="rootTransform">Трансформ Root.</param>
    /// <param name="hipsTransform">Трансформ J_Bip_C_Hips.</param>
    private void AdjustObjectPositions(GameObject mainObject, Transform rootTransform, Transform hipsTransform)
    {
        // Сохраняем текущее смещение J_Bip_C_Hips
        Vector3 hipsOffset = hipsTransform.localPosition;

        // Меняем знак смещения для оси Y
        hipsOffset.y = -hipsOffset.y;

        // Обнуляем позицию J_Bip_C_Hips
        hipsTransform.localPosition = Vector3.zero;

        // Применяем смещение ко всем объектам внутри mainObject, кроме Root и его содержимого
        foreach (Transform child in mainObject.transform)
        {
            if (child == rootTransform || child.IsChildOf(rootTransform))
                continue;

            child.localPosition += hipsOffset;
        }

        mainObject.transform.position -= new Vector3(0, hipsOffset.y, 0);
    }

    private Transform FindBone(string partialName)
    {
        foreach (Transform bone in GetComponentsInChildren<Transform>())
        {
            if (bone.name.Contains(partialName))
                return bone;
        }
        return null;
    }

    // private void AddRigidbodyAndJoint(Transform bone, Transform parent, float mass)
    // {
    //     if (bone == null) return;

    //     Rigidbody rb = bone.gameObject.AddComponent<Rigidbody>();
    //     rb.mass = mass;

    //     if (parent != null)
    //     {
    //         CharacterJoint joint = bone.gameObject.AddComponent<CharacterJoint>();
    //         joint.connectedBody = parent.GetComponent<Rigidbody>();
    //     }
    // }

    private void AddRigidbodyAndJoint(Transform bone, Transform parent, BoneInfo boneInfo)
    {
        if (bone == null) return;

        // Настраиваем Rigidbody
        Rigidbody rb = bone.gameObject.AddComponent<Rigidbody>();
        rb.mass = boneInfo.mass;
        rb.linearDamping = boneInfo.linearDamping; // Заменяем drag
        rb.angularDamping = boneInfo.angularDamping; // Заменяем angularDrag

        if (parent != null)
        {
            // Настраиваем CharacterJoint
            CharacterJoint joint = bone.gameObject.AddComponent<CharacterJoint>();
            joint.connectedBody = parent.GetComponent<Rigidbody>();
            joint.breakForce = boneInfo.breakForce;
            joint.breakTorque = boneInfo.breakTorque;
            joint.enableCollision = true;

            // Устанавливаем ограничения
            if (boneInfo.useLimits)
            {
                SoftJointLimit swing1 = new SoftJointLimit { limit = boneInfo.swing1Limit };
                SoftJointLimit swing2 = new SoftJointLimit { limit = boneInfo.swing2Limit };
                SoftJointLimit twistLow = new SoftJointLimit { limit = -boneInfo.twistLimit }; // Отрицательный предел
                SoftJointLimit twistHigh = new SoftJointLimit { limit = boneInfo.twistLimit }; // Положительный предел

                SoftJointLimitSpring limitSpring = new SoftJointLimitSpring
                {
                    spring = 0f, // Добавьте пружинное сопротивление, если нужно
                    damper = 0f // Добавьте демпфирование, если нужно
                };

                joint.swing1Limit = swing1;
                joint.swing2Limit = swing2;
                joint.lowTwistLimit = twistLow;
                joint.highTwistLimit = twistHigh;
                joint.swingLimitSpring = limitSpring;
            }

            // Смещение соединения
            joint.connectedAnchor += boneInfo.connectedAnchorOffset;
        }
    }
}