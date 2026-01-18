using UnityEngine;

public class LegAdjuster : MonoBehaviour
{
    public float legOffset = 0.03f; // Насколько раздвигаем ноги
    private Transform leftLeg;
    private Transform rightLeg;
    private Vector3 leftLegDefaultPos;
    private Vector3 rightLegDefaultPos;

    void Awake()
    {
        FindLegBones();
    }

    void LateUpdate()
    {
        AdjustLegPositions();
    }

    private void FindLegBones()
    {
        leftLeg = FindChildByName(transform, "J_Bip_L_UpperLeg");
        rightLeg = FindChildByName(transform, "J_Bip_R_UpperLeg");
        
        if (leftLeg != null) leftLegDefaultPos = leftLeg.localPosition;
        if (rightLeg != null) rightLegDefaultPos = rightLeg.localPosition;
    }

    private void AdjustLegPositions()
    {
        if (leftLeg != null)
            leftLeg.localPosition = leftLegDefaultPos + new Vector3(-legOffset, 0, 0);

        if (rightLeg != null)
            rightLeg.localPosition = rightLegDefaultPos + new Vector3(legOffset, 0, 0);
    }

    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildByName(child, name);
            if (found != null) return found;
        }
        return null;
    }
}



// это ломает модель
// using UnityEngine;

// public class LegAdjuster : MonoBehaviour
// {
//     public Transform leftLeg;
//     public Transform rightLeg;
//     public float offset = 0.3f; // Насколько раздвинуть

//     private SkinnedMeshRenderer skinnedMeshRenderer;
//     private Mesh mesh;

//     void Start()
//     {
//         skinnedMeshRenderer = transform.Find("Body").GetComponentInChildren<SkinnedMeshRenderer>();
//         if (skinnedMeshRenderer == null) return;

//         mesh = skinnedMeshRenderer.sharedMesh;
//         if (mesh == null) return;

//         Matrix4x4[] newBindPoses = mesh.bindposes;

//         // Ищем индексы костей в массиве
//         int leftLegIndex = FindBoneIndex(leftLeg);
//         int rightLegIndex = FindBoneIndex(rightLeg);

//         if (leftLegIndex != -1)
//         {
//             newBindPoses[leftLegIndex] *= Matrix4x4.Translate(new Vector3(-offset, 0, 0));
//         }

//         if (rightLegIndex != -1)
//         {
//             newBindPoses[rightLegIndex] *= Matrix4x4.Translate(new Vector3(offset, 0, 0));
//         }

//         mesh.bindposes = newBindPoses;
//         skinnedMeshRenderer.sharedMesh = mesh;
//     }

//     int FindBoneIndex(Transform bone)
//     {
//         for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
//         {
//             if (skinnedMeshRenderer.bones[i] == bone)
//                 return i;
//         }
//         return -1;
//     }
// }



