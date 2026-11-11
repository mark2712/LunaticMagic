namespace States
{
    public class SneakSlow : BaseSneak
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.SneakSlow();
        }
    }
}