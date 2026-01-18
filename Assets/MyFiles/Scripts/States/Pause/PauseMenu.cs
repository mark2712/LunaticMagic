namespace States
{
    public class PauseMenu : PauseBase
    {
        public PauseMenu() : base()
        {
            RegisterEvent(StateEvent.ConsolePerformed, (state, i) =>
            {
                SM.ReturnToLayer();
                return null;
            });

            RegisterEvent(StateEvent.EscPerformed, (state, i) =>
            {
                SM.ReturnToLayer();
                return null;
            });
        }

        public override void Enter()
        {
            base.Enter();
            // GameContext.UIManager.TogglePauseMenu(true);
        }

        public override void Exit()
        {
            base.Exit();
            // GameContext.UIManager.TogglePauseMenu(false);
        }
    }
}