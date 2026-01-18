using System;
using Mobs.Physics;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Environment
{
    // решает, какое тело нужно на основе данных о нахождении в текущих средах в EnvironmentData
    public class PlayerBodySwitcher : IEnvironmentBodySwitcher
    {
        public GameObject MobGameObject { get; protected set; }

        public PlayerBodySwitcher(GameObject mobGameObject)
        {
            MobGameObject = mobGameObject;
        }

        public IPhysicsBody GetPhysicsBody(EnvironmentData EnvironmentData)
        {
            // if (ctx.OnCable) return cableBody;
            // if (ctx.InWater) return waterBody;
            // if (ctx.HasGround && ctx.GroundAngle <= 60) return groundBody;
            // return airBody;
            return new GroundPlayerPhysicsBody(MobGameObject);
        }
    }
}
