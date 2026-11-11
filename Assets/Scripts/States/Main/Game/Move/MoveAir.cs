namespace States
{
    public class MoveAir : Air
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.EmotionController.SetEmotion("Surprised", 50f);
        }

        public override void Exit()
        {
            base.Exit();
            GameContext.EmotionController.SetEmotion("Neutral", 800f);
        }
    }
}