using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class BaseGame : MainBase
    {
        public override bool Reentry => false;

        public BaseGame() : base()
        {
            // RegisterEvent(StateEvent.MyEvent, (state, i) =>
            // {
            //     Debug.Log("MyEvent in BaseGame");
            //     InvokeEvent(StateEvent.MyEvent, i - 1);
            //     return null;
            // });

            RegisterEvent(StateEvent.Mouse1Performed, (state, i) => { return SMController.HandsSM.InvokeEvent(StateEvent.Mouse1Performed); });
            RegisterEvent(StateEvent.Mouse2Performed, (state, i) => { return SMController.HandsSM.InvokeEvent(StateEvent.Mouse2Performed); });

            RegisterEvent(StateEvent.SpacePerformed, (state, i) => { return new Jump(); });
            RegisterEvent(StateEvent.TabPerformed, (state, i) => { return SMController.HandsSM.InvokeEvent(StateEvent.TabPerformed); });
            RegisterEvent(StateEvent.ConsolePerformed, (state, i) => { SM.GoToLayer(new Console()); return null; });

            RegisterEvent(StateEvent.MoveChanged, (state, i) => { return SM.GetGameState(); });
            RegisterEvent(StateEvent.GroundChanged, (state, i) => { return SM.GetGameState(); });
            RegisterEvent(StateEvent.ShiftChanged, (state, i) => { return SM.GetGameState(); });
            RegisterEvent(StateEvent.SneakChanged, (state, i) => { return SM.GetGameState(); });
            RegisterEvent(StateEvent.LegsRopeChanged, (state, i) => { return SMController.LegsSM.InvokeEvent(StateEvent.LegsRopeChanged); });
            RegisterEvent(StateEvent.HandsRopeChanged, (state, i) => { return SMController.HandsSM.InvokeEvent(StateEvent.HandsRopeChanged); });

            RegisterEvent(StateEvent.KeyI, (state, i) => { CounterFPS.Inc(); return null; });
            RegisterEvent(StateEvent.KeyZ, (state, i) => { GameContext.CamerasController.ChangeActiveCameraThirdFirstPerson(); return null; });

            RegisterEvent(StateEvent.F1, (state, i) => { return SMController.HandsSM.InvokeEvent(StateEvent.F1); });
            RegisterEvent(StateEvent.F2, (state, i) => { return SMController.LegsSM.InvokeEvent(StateEvent.F2); });

            RegisterEventsShiftSneak();
        }

        public override void Enter()
        {
            // GameContext.PlayerController.NowMoveSpeed = PlayerSpeed.Get();
        }

        public override void Update()
        {
            GameContext.CameraPlayerController.Update();
            GameContext.PlayerController.SetMoveInput();
        }

        //Inputs
        public override State ScrollPerformed(InputAction.CallbackContext ctx)
        {
            GameContext.CameraPlayerController.OnScrollInputPerformed(ctx);
            return null;
        }
    }
}