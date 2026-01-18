using System;
using System.Collections.Generic;
using Environment;
using UnityEngine;

namespace Mobs.Physics
{
    public abstract class BasePhysicsBody : IPhysicsBody, ICanMovePhysicsBody, ICanJumpPhysicsBody
    {
        public IEnvironmentDetector EnvironmentDetector { get; protected set; }
        public IScenePhysicsBody ScenePhysicsBody { get; protected set; }

        // Словарь обработчиков команд: Тип команды -> Действие
        // protected readonly Dictionary<Type, Action<IBodyCommand>> CommandHandlers = new();

        public float MoveSpeed { get; set; }
        public float JumpForce { get; set; }
        public float GravityScale { get; set; }
        public Vector3 MoveInput { get; set; }

        public BasePhysicsBody(GameObject mobGameObject)
        {
            ScenePhysicsBody = new PlayerScenePhysicsBody(mobGameObject);
            EnvironmentDetector = new EnvironmentDetector(ScenePhysicsBody);
        }

        public virtual void Activate() { ScenePhysicsBody.Activate(); }
        public virtual void Deactivate() { ScenePhysicsBody.Deactivate(); }

        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public virtual void FixedUpdate()
        {
            EnvironmentDetector.FixedUpdate();
        }

        public void Move(Vector3 moveInput)
        {
            MoveInput = moveInput;
        }

        public void Jump()
        {
            // говорим что нужно сделать прыжок в ближайшем кадре FixedUpdate
        }
    }
}