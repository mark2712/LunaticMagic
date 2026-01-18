namespace States
{
    public class PunchR : BaseHandsHit
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.HandsPunchRight();
            StartTimer(900);
            RegisterEvent(StateEvent.HitFinished, (state, i) =>
            {
                return HitFinished();
            });
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            GameContext.CameraPlayerController.Update();
            GameContext.PlayerController.SetMoveInput(); // можно немного сместиться в выбранном направлении

            if (IsTimerFinished())
            {
                SM.TriggerEvent(StateEvent.HitFinished);
            }
        }

        private State HitFinished()
        {
            if (GameContext.InputActions.Player.Mouse1.IsPressed())
            {
                return new PunchR();
            }
            return SM.GetHandsState();
        }
    }
}