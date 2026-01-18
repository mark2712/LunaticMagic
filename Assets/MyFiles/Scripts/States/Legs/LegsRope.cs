namespace States
{
    public class LegsRope : BaseLegs
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.LegsRope();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}