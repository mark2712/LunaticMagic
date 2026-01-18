using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace InputLayer
{
    public sealed class InputController : IInputController
    {
        public PlayerInputActions InputActions { get; private set; } = new();

        public AxisInput<Vector2> Move { get; } = new(Vector2.zero);
        public AxisInput<Vector2> Look { get; } = new(Vector2.zero);
        public AxisInput<float> Scroll { get; } = new(0f);

        private readonly Dictionary<Inputs, AxisInput<bool>> buttons = new();
        public AxisInput<bool> GetButton(Inputs input) => buttons[input];


        public InputController()
        {
            InputActions.Player.Enable();
            Bind();
        }

        private void Bind()
        {
            InputActions.Player.Move.performed += ctx =>
                Move.SetValue(ctx.ReadValue<Vector2>());

            InputActions.Player.Move.canceled += _ =>
                Move.SetValue(Vector2.zero);

            InputActions.Player.Look.performed += ctx =>
                Look.SetValue(ctx.ReadValue<Vector2>());

            InputActions.Player.Look.canceled += _ =>
                Look.SetValue(Vector2.zero);

            InputActions.Player.Scroll.performed += ctx =>
            {
                float delta = ctx.ReadValue<Vector2>().y;
                if (Mathf.Abs(delta) > 0f)
                    Scroll.SetValue(delta);
            };

            // Кнопки
            foreach (Inputs i in Enum.GetValues(typeof(Inputs)))
            {
                if (i is Inputs.Move or Inputs.Look or Inputs.Scroll)
                    continue;

                buttons[i] = new AxisInput<bool>(false);
            }

            BindButtons();
        }

        private void BindButton(InputAction action, Inputs input)
        {
            action.performed += _ => buttons[input].SetValue(true);
            action.canceled += _ => buttons[input].SetValue(false);
        }

        public void LateUpdate()
        {
            Scroll.SetValue(0f); // Scroll — импульс
        }

        private void BindButtons()
        {
            // Мышь
            BindButton(InputActions.Player.Mouse1, Inputs.Mouse1);
            BindButton(InputActions.Player.Mouse2, Inputs.Mouse2);
            BindButton(InputActions.Player.Mouse3, Inputs.Mouse3);

            // Модификаторы
            BindButton(InputActions.Player.Shift, Inputs.Shift);
            BindButton(InputActions.Player.Ctrl, Inputs.Ctrl);
            BindButton(InputActions.Player.Alt, Inputs.Alt);

            // Основные
            BindButton(InputActions.Player.Space, Inputs.Space);
            BindButton(InputActions.Player.Tab, Inputs.Tab);
            BindButton(InputActions.Player.Esc, Inputs.Esc);
            BindButton(InputActions.Player.Console, Inputs.Console);

            // Буквы
            BindButton(InputActions.Player.Q, Inputs.Q);
            BindButton(InputActions.Player.E, Inputs.E);
            BindButton(InputActions.Player.R, Inputs.R);
            BindButton(InputActions.Player.T, Inputs.T);
            BindButton(InputActions.Player.I, Inputs.I);
            BindButton(InputActions.Player.F, Inputs.F);
            BindButton(InputActions.Player.Z, Inputs.Z);
            BindButton(InputActions.Player.X, Inputs.X);
            BindButton(InputActions.Player.C, Inputs.C);

            // Цифры
            BindButton(InputActions.Player.Num0, Inputs.Num0);
            BindButton(InputActions.Player.Num1, Inputs.Num1);
            BindButton(InputActions.Player.Num2, Inputs.Num2);
            BindButton(InputActions.Player.Num3, Inputs.Num3);
            BindButton(InputActions.Player.Num4, Inputs.Num4);
            BindButton(InputActions.Player.Num5, Inputs.Num5);
            BindButton(InputActions.Player.Num6, Inputs.Num6);
            BindButton(InputActions.Player.Num7, Inputs.Num7);
            BindButton(InputActions.Player.Num8, Inputs.Num8);
            BindButton(InputActions.Player.Num9, Inputs.Num9);

            // F-клавиши
            BindButton(InputActions.Player.F1, Inputs.F1);
            BindButton(InputActions.Player.F2, Inputs.F2);
            BindButton(InputActions.Player.F3, Inputs.F3);
            BindButton(InputActions.Player.F4, Inputs.F4);
            BindButton(InputActions.Player.F5, Inputs.F5);
            BindButton(InputActions.Player.F6, Inputs.F6);
            BindButton(InputActions.Player.F7, Inputs.F7);
            BindButton(InputActions.Player.F8, Inputs.F8);
            BindButton(InputActions.Player.F9, Inputs.F9);
            BindButton(InputActions.Player.F10, Inputs.F10);
            BindButton(InputActions.Player.F11, Inputs.F11);
            BindButton(InputActions.Player.F12, Inputs.F12);
        }
    }
}
