using UnityEngine;

namespace States
{
    public class Move : BaseMove
    {
        protected MoveNormalAnimation moveNormalAnimation = new();

        public override void Enter()
        {
            base.Enter();
            moveNormalAnimation.Update();
        }

        public override void Update()
        {
            base.Update();
            moveNormalAnimation.Update();
        }

        // public override State MoveInput(Vector2 moveInput)
        // {
        //     // base.MoveInput(moveInput);
        //     this.moveInput = moveInput;
        //     return null;
        // }
    }
}