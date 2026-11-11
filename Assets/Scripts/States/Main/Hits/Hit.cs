namespace States
{
    public class Hit : HitBase
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.Kick();
            StartTimer(1200);
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
                return new Hit();
            }
            return SM.GetGameState();
        }
    }
}