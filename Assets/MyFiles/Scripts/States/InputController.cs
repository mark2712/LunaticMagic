using UnityEngine;


namespace States
{
    public class InputController
    {
        private readonly SM mainSM;
        public Vector2 MoveInput = Vector2.zero;
        public Vector2 LookInput = Vector2.zero;

        public InputController(SM mainSM)
        {
            this.mainSM = mainSM;
        }

        public void SetMoveInput(Vector2 moveInput) { MoveInput = moveInput; }
        public void SetLookInput(Vector2 lookInput) { LookInput = lookInput; }

        public void InitializeInput(PlayerInputActions inputActions)
        {
            inputActions.Player.Scroll.performed += ctx => mainSM.ScrollPerformed(ctx); // Подписываемся на событие колесика мыши

            inputActions.Player.Esc.performed += _ => mainSM.TriggerEvent(StateEvent.EscPerformed);
            inputActions.Player.Console.performed += _ => mainSM.TriggerEvent(StateEvent.ConsolePerformed);

            inputActions.Player.Move.performed += ctx => SetMoveInput(ctx.ReadValue<Vector2>());
            inputActions.Player.Move.canceled += ctx => SetMoveInput(Vector2.zero);

            inputActions.Player.Look.performed += ctx => SetLookInput(ctx.ReadValue<Vector2>());
            inputActions.Player.Look.canceled += ctx => SetLookInput(Vector2.zero);

            inputActions.Player.Mouse1.performed += _ => mainSM.TriggerEvent(StateEvent.Mouse1Performed);
            inputActions.Player.Mouse2.performed += _ => mainSM.TriggerEvent(StateEvent.Mouse2Performed);
            inputActions.Player.Mouse3.performed += _ => mainSM.TriggerEvent(StateEvent.Mouse3Performed);

            inputActions.Player.Tab.performed += _ => mainSM.TriggerEvent(StateEvent.TabPerformed);

            inputActions.Player.Shift.performed += _ => mainSM.TriggerEvent(StateEvent.ShiftPerformed);
            inputActions.Player.Shift.canceled += _ => mainSM.TriggerEvent(StateEvent.ShiftCanceled);

            inputActions.Player.Ctrl.performed += _ => mainSM.TriggerEvent(StateEvent.CtrlPerformed);
            inputActions.Player.Ctrl.canceled += _ => mainSM.TriggerEvent(StateEvent.CtrlCanceled);

            inputActions.Player.Alt.performed += _ => mainSM.TriggerEvent(StateEvent.AltPerformed);
            inputActions.Player.Alt.canceled += _ => mainSM.TriggerEvent(StateEvent.AltCanceled);

            inputActions.Player.Space.performed += _ => mainSM.TriggerEvent(StateEvent.SpacePerformed);

            inputActions.Player.Q.performed += _ => mainSM.TriggerEvent(StateEvent.KeyQ);
            inputActions.Player.E.performed += _ => mainSM.TriggerEvent(StateEvent.KeyE);
            inputActions.Player.R.performed += _ => mainSM.TriggerEvent(StateEvent.KeyR);
            inputActions.Player.T.performed += _ => mainSM.TriggerEvent(StateEvent.KeyT);
            inputActions.Player.I.performed += _ => mainSM.TriggerEvent(StateEvent.KeyI);
            inputActions.Player.F.performed += _ => mainSM.TriggerEvent(StateEvent.KeyF);
            inputActions.Player.Z.performed += _ => mainSM.TriggerEvent(StateEvent.KeyZ);
            inputActions.Player.X.performed += _ => mainSM.TriggerEvent(StateEvent.KeyX);
            inputActions.Player.C.performed += _ => mainSM.TriggerEvent(StateEvent.KeyC);

            inputActions.Player.Num1.performed += _ => mainSM.TriggerEvent(StateEvent.Num1);
            inputActions.Player.Num2.performed += _ => mainSM.TriggerEvent(StateEvent.Num2);
            inputActions.Player.Num3.performed += _ => mainSM.TriggerEvent(StateEvent.Num3);
            inputActions.Player.Num4.performed += _ => mainSM.TriggerEvent(StateEvent.Num4);
            inputActions.Player.Num5.performed += _ => mainSM.TriggerEvent(StateEvent.Num5);
            inputActions.Player.Num6.performed += _ => mainSM.TriggerEvent(StateEvent.Num6);
            inputActions.Player.Num7.performed += _ => mainSM.TriggerEvent(StateEvent.Num7);
            inputActions.Player.Num8.performed += _ => mainSM.TriggerEvent(StateEvent.Num8);
            inputActions.Player.Num9.performed += _ => mainSM.TriggerEvent(StateEvent.Num9);
            inputActions.Player.Num0.performed += _ => mainSM.TriggerEvent(StateEvent.Num0);

            inputActions.Player.F1.performed += _ => mainSM.TriggerEvent(StateEvent.F1);
            inputActions.Player.F2.performed += _ => mainSM.TriggerEvent(StateEvent.F2);
            inputActions.Player.F3.performed += _ => mainSM.TriggerEvent(StateEvent.F3);
            inputActions.Player.F4.performed += _ => mainSM.TriggerEvent(StateEvent.F4);
            inputActions.Player.F5.performed += _ => mainSM.TriggerEvent(StateEvent.F5);
            inputActions.Player.F6.performed += _ => mainSM.TriggerEvent(StateEvent.F6);
            inputActions.Player.F7.performed += _ => mainSM.TriggerEvent(StateEvent.F7);
            inputActions.Player.F8.performed += _ => mainSM.TriggerEvent(StateEvent.F8);
            inputActions.Player.F9.performed += _ => mainSM.TriggerEvent(StateEvent.F9);
            inputActions.Player.F10.performed += _ => mainSM.TriggerEvent(StateEvent.F10);
            inputActions.Player.F11.performed += _ => mainSM.TriggerEvent(StateEvent.F11);
            inputActions.Player.F12.performed += _ => mainSM.TriggerEvent(StateEvent.F12);
        }
    }
}