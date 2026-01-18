using UnityEngine;

namespace Mobs.Physics
{
    public struct MoveCommand : IBodyCommand
    {
        public Vector3 MoveInput;

        public void ExecuteCommand(IPhysicsBody body)
        {
            if (body is ICanMovePhysicsBody movable) { movable.Move(MoveInput); }
        }
    }

    public interface ICanMovePhysicsBody
    {
        void Move(Vector3 direction);
    }

    public struct JumpCommand : IBodyCommand
    {
        public void ExecuteCommand(IPhysicsBody body)
        {
            if (body is ICanJumpPhysicsBody jumpable) { jumpable.Jump(); }
        }
    }

    public interface ICanJumpPhysicsBody
    {
        void Jump();
    }
}