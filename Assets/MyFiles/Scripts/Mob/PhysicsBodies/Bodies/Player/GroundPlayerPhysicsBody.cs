using UnityEngine;

namespace Mobs.Physics
{
    public class GroundPlayerPhysicsBody : BasePhysicsBody
    {
        public GroundPlayerPhysicsBody(GameObject mobGameObject) : base(mobGameObject) { }

        public override void FixedUpdate()
        {
            // реализация движения
        }

        // public override void Jump()
        // {
        //     // реализация прыжка
        // }

        // protected override void SetupCommands()
        // {
        //     // Регистрируем обработку перемещения
        //     CommandHandlers[typeof(MoveCommand)] = (cmd) =>
        //     {
        //         var move = (MoveCommand)cmd;
        //         // _moveDirection = move.Direction;
        //     };

        //     // Регистрируем прыжок
        //     CommandHandlers[typeof(JumpCommand)] = (cmd) =>
        //     {
        //         ScenePhysicsBody.MobRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        //     };
        // }
    }
}