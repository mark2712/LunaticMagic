using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class HitBase : State
    {
        public override SM SM => SMController.MainSM;

        public override bool Reentry => true;

        public HitBase() : base()
        {
            RegisterEvent(StateEvent.Mouse1Performed, (state, i) => { return null; });
            RegisterEvent(StateEvent.SpacePerformed, (state, i) => { return null; });
            RegisterEvent(StateEvent.ConsolePerformed, (state, i) =>
            {
                SM.GoToLayer(new Console());
                return null;
            });
            RegisterEventsShiftSneak();
        }

        public override void Enter()
        {
            Flags.Set(FlagName.Hit, true);
        }

        public override void Update()
        {
            GameContext.CameraPlayerController.Update();
            GameContext.PlayerController.SetMoveInput();
        }

        public override void Exit()
        {
            Flags.Set(FlagName.Hit, false);
        }

        //Inputs
        public override State ScrollPerformed(InputAction.CallbackContext ctx)
        {
            GameContext.CameraPlayerController.OnScrollInputPerformed(ctx);
            return null;
        }
    }
}