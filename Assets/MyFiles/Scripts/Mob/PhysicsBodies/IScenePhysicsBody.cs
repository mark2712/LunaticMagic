using UnityEngine;

namespace Mobs.Physics
{
    public interface IScenePhysicsBody
    {
        GameObject MobGameObject { get; }
        Rigidbody MobRigidbody { get; }
        void Activate();
        void Deactivate();
    }
}

// void EnableVerticalCapsule();
// void EnableHorizontalCapsule();
// void DisableAllColliders();

// void SetFeetMaterial(PhysicMaterial physicMaterial);
// void SetGravityScale(float scale);

// void ApplyVelocity(Vector3 velocity);