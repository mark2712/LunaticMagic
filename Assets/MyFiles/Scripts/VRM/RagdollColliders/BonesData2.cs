using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BonesData2
{
    [System.Serializable]
    public class BoneInfo
    {
        public string name;
        public string parentName;
        public float mass;
        public float swing1Limit;
        public float swing2Limit;
        public float twistLimit;
        public float linearDamping; // Новое поле
        public float angularDamping; // Новое поле
        public float breakForce;
        public float breakTorque;
        public bool useLimits;
        public Vector3 connectedAnchorOffset;

        // Конструктор для инициализации всех полей
        public BoneInfo(
            string name,
            string parentName,
            float mass,
            float swing1Limit,
            float swing2Limit,
            float twistLimit,
            float linearDamping,
            float angularDamping,
            float breakForce,
            float breakTorque,
            bool useLimits,
            Vector3 connectedAnchorOffset)
        {
            this.name = name;
            this.parentName = parentName;
            this.mass = mass;
            this.swing1Limit = swing1Limit;
            this.swing2Limit = swing2Limit;
            this.twistLimit = twistLimit;
            this.linearDamping = linearDamping;
            this.angularDamping = angularDamping;
            this.breakForce = breakForce;
            this.breakTorque = breakTorque;
            this.useLimits = useLimits;
            this.connectedAnchorOffset = connectedAnchorOffset;
        }
    }

    public List<BoneInfo> boneParameters = new List<BoneInfo>
    {
        new BoneInfo
        (
            name: "Root",
            parentName: null,
            mass: 12f,
            swing1Limit: 45f,
            swing2Limit: 45f,
            twistLimit: 30f,
            linearDamping: 0.6f,
            angularDamping: 0.2f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo(
            name: "J_Bip_C_Hips",
            parentName: "Root",
            mass: 10f,
            swing1Limit: 30f,
            swing2Limit: 30f,
            twistLimit: 15f,
            linearDamping: 0.5f, // Значение для linearDamping
            angularDamping: 0.1f, // Значение для angularDamping
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_C_Spine",
            parentName: "J_Bip_C_Hips",
            mass: 8f,
            swing1Limit: 30f,
            swing2Limit: 30f,
            twistLimit: 20f,
            linearDamping: 0.5f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_C_Chest",
            parentName: "J_Bip_C_Spine",
            mass: 6f,
            swing1Limit: 25f,
            swing2Limit: 25f,
            twistLimit: 15f,
            linearDamping: 0.4f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_C_Neck",
            parentName: "J_Bip_C_Chest",
            mass: 4f,
            swing1Limit: 20f,
            swing2Limit: 20f,
            twistLimit: 10f,
            linearDamping: 0.3f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_C_Head",
            parentName: "J_Bip_C_Neck",
            mass: 3f,
            swing1Limit: 15f,
            swing2Limit: 15f,
            twistLimit: 10f,
            linearDamping: 0.2f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),

        new BoneInfo
        (
            name: "J_Bip_L_UpperArm",
            parentName: "J_Bip_C_Chest",
            mass: 3f,
            swing1Limit: 40f,
            swing2Limit: 40f,
            twistLimit: 30f,
            linearDamping: 0.4f,
            angularDamping: 0.15f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_L_LowerArm",
            parentName: "J_Bip_L_UpperArm",
            mass: 2f,
            swing1Limit: 60f,
            swing2Limit: 30f,
            twistLimit: 25f,
            linearDamping: 0.3f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_L_Hand",
            parentName: "J_Bip_L_LowerArm",
            mass: 1f,
            swing1Limit: 70f,
            swing2Limit: 40f,
            twistLimit: 20f,
            linearDamping: 0.2f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_R_UpperArm",
            parentName: "J_Bip_C_Chest",
            mass: 3f,
            swing1Limit: 40f,
            swing2Limit: 40f,
            twistLimit: 30f,
            linearDamping: 0.4f,
            angularDamping: 0.15f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_R_LowerArm",
            parentName: "J_Bip_R_UpperArm",
            mass: 2f,
            swing1Limit: 60f,
            swing2Limit: 30f,
            twistLimit: 25f,
            linearDamping: 0.3f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_R_Hand",
            parentName: "J_Bip_R_LowerArm",
            mass: 1f,
            swing1Limit: 70f,
            swing2Limit: 40f,
            twistLimit: 20f,
            linearDamping: 0.2f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_L_UpperLeg",
            parentName: "J_Bip_C_Hips",
            mass: 5f,
            swing1Limit: 45f,
            swing2Limit: 30f,
            twistLimit: 20f,
            linearDamping: 0.4f,
            angularDamping: 0.15f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_L_LowerLeg",
            parentName: "J_Bip_L_UpperLeg",
            mass: 4f,
            swing1Limit: 60f,
            swing2Limit: 15f,
            twistLimit: 15f,
            linearDamping: 0.3f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_L_Foot",
            parentName: "J_Bip_L_LowerLeg",
            mass: 2f,
            swing1Limit: 15f, // Ограничение размаха наружу
            swing2Limit: 15f, // Ограничение размаха наружу
            twistLimit: 20f,  // Увеличен диапазон поворота внутрь
            linearDamping: 0.5f, // Больше сопротивления линейным движениям
            angularDamping: 0.1f, // Больше сопротивления вращению
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: new Vector3(-0.05f, 0f, 0f) // Легкое смещение внутрь
        ),
        new BoneInfo
        (
            name: "J_Bip_R_UpperLeg",
            parentName: "J_Bip_C_Hips",
            mass: 5f,
            swing1Limit: 45f,
            swing2Limit: 30f,
            twistLimit: 20f,
            linearDamping: 0.4f,
            angularDamping: 0.15f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_R_LowerLeg",
            parentName: "J_Bip_R_UpperLeg",
            mass: 4f,
            swing1Limit: 60f,
            swing2Limit: 15f,
            twistLimit: 15f,
            linearDamping: 0.3f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: Vector3.zero
        ),
        new BoneInfo
        (
            name: "J_Bip_R_Foot",
            parentName: "J_Bip_R_LowerLeg",
            mass: 2f,
            swing1Limit: 15f,
            swing2Limit: 15f,
            twistLimit: 20f,
            linearDamping: 0.5f,
            angularDamping: 0.1f,
            breakForce: Mathf.Infinity,
            breakTorque: Mathf.Infinity,
            useLimits: true,
            connectedAnchorOffset: new Vector3(0.05f, 0f, 0f) // Легкое смещение внутрь 
        )
    };

    // public List<BoneInfo> boneParameters = new List<BoneInfo>
    // {
    //     new BoneInfo { name = "Root", parentName = null, mass = 12f },

    //     new BoneInfo { name = "J_Bip_C_Hips", parentName = "Root", mass = 10f },
    //     new BoneInfo { name = "J_Bip_C_Spine", parentName = "J_Bip_C_Hips", mass = 8f },
    //     new BoneInfo { name = "J_Bip_C_Chest", parentName = "J_Bip_C_Spine", mass = 6f },
    //     new BoneInfo { name = "J_Bip_C_Neck", parentName = "J_Bip_C_Chest", mass = 4f },
    //     new BoneInfo { name = "J_Bip_C_Head", parentName = "J_Bip_C_Neck", mass = 3f },

    //     new BoneInfo { name = "J_Bip_L_UpperArm", parentName = "J_Bip_C_Chest", mass = 3f },
    //     new BoneInfo { name = "J_Bip_L_LowerArm", parentName = "J_Bip_L_UpperArm", mass = 2f },
    //     new BoneInfo { name = "J_Bip_L_Hand", parentName = "J_Bip_L_LowerArm", mass = 1f },

    //     new BoneInfo { name = "J_Bip_R_UpperArm", parentName = "J_Bip_C_Chest", mass = 3f },
    //     new BoneInfo { name = "J_Bip_R_LowerArm", parentName = "J_Bip_R_UpperArm", mass = 2f },
    //     new BoneInfo { name = "J_Bip_R_Hand", parentName = "J_Bip_R_LowerArm", mass = 1f },

    //     new BoneInfo { name = "J_Bip_L_UpperLeg", parentName = "J_Bip_C_Hips", mass = 5f },
    //     new BoneInfo { name = "J_Bip_L_LowerLeg", parentName = "J_Bip_L_UpperLeg", mass = 4f },
    //     new BoneInfo { name = "J_Bip_L_Foot", parentName = "J_Bip_L_LowerLeg", mass = 2f },

    //     new BoneInfo { name = "J_Bip_R_UpperLeg", parentName = "J_Bip_C_Hips", mass = 5f },
    //     new BoneInfo { name = "J_Bip_R_LowerLeg", parentName = "J_Bip_R_UpperLeg", mass = 4f },
    //     new BoneInfo { name = "J_Bip_R_Foot", parentName = "J_Bip_R_LowerLeg", mass = 2f }
    // };
}