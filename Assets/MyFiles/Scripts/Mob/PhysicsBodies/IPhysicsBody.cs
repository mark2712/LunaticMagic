using System;
using Environment;
using UnityEngine;

namespace Mobs.Physics
{
    public interface IPhysicsBody
    {
        IScenePhysicsBody ScenePhysicsBody { get; }
        IEnvironmentDetector EnvironmentDetector { get; }

        void Activate();
        void Deactivate();

        void FixedUpdate();
        void Update();
        void LateUpdate();
    }
}