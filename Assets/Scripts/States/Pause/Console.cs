using UnityEngine;

namespace States
{
    public class Console : PauseBase
    {
        public override SM SM => SMController.MainSM;

        public Console() : base()
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
            // GameContext.UIManager.ToggleConsole(true);
        }

        public override void Exit()
        {
            base.Exit();
            // GameContext.UIManager.ToggleConsole(false);
        }
    }
}