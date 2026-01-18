namespace States
{
    public class SneakStand : BaseSneak
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.Stand();
            GameContext.EmotionController.SetEmotion("Neutral", 800f);
        }
    }
}