namespace States
{
    public class LegsFree : BaseLegs
    {
        public LegsFree() : base()
        {
            RegisterEvent(StateEvent.Mouse1Performed, (state, i) => { return new Hit(); });
        }

        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.LegsNone();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}