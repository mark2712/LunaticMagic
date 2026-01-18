using UnityEngine;

namespace States
{
    public class Stand : BaseMove
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.Stand();
            GameContext.EmotionController.SetEmotion("Neutral", 800f);
        }
    }
}