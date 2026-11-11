using System;
using EnvironmentOld;
using UnityEngine;

namespace MobsOld
{
    public class Body
    {
        public EnvironmentState EnvironmentState { get; set; } = EnvironmentState.Ground;
        public IPhysicsController PhysicsController { get; set; }
        public Transform Model { get; set; }

        public void Stand() { }
        public void Sit() { }
        public void Lie() { }
        public void Swim() { }

        public void Glide() { }
        public void NoGlide() { }
        public void Windage() { }

    }

    public interface IPhysicsController
    {
        float NowMoveSpeed { get; set; }
        event Action<bool> OnGroundChanged;
        bool IsGround { get; } // персонаж на земле (работает во всех средах)
        Vector3 MoveInput { get; set; } // если персонаж ходит по земле то учитывается только двуменая состовляющая
        void SyncCamera(); // камера синхронизируется с направлением движения персонажа
        void Jump(); // прыжок
        void FixedUpdate() { }
        void Update() { }
        void LateUpdate() { }
    }
}