using UnityEngine;

namespace States
{
    public class Air : BaseAir
    {
        public Air()
        {
            RegisterEvent(StateEvent.SpacePerformed, (state, i) =>
            {
                // Player.PlayerController playerController = GameContext.playerController;
                // if (playerController.JumpCount < 2 && Time.time - playerController.LastJumpTime > 0.1f && playerController.DoubleJumpOn)
                // {
                //     GoToState<DoubleJump>();
                // }
                return null;
            });
        }

        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.Air();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}