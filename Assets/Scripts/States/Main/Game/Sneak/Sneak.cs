using UnityEngine;

namespace States
{
    public class Sneak : BaseSneak
    {
        protected MoveSlowAnimation moveSlowAnimation = new();
        public override void Enter()
        {
            base.Enter();
            moveSlowAnimation.Update();
            // GameContext.playerAnimationController.Sneak();
        }

        public override void Update()
        {
            base.Update();
            GameContext.CameraPlayerController.Update();
            moveSlowAnimation.Update();
        }
    }
}