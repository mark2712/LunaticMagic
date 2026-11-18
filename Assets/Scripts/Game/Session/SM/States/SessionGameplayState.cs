using System;
using Entities;
using Game.Session.Sound;
using LocalTime;
using UniRx;
using UnityEngine;

namespace SessionSM
{
    // Активный игровой мир (геймплей)
    // Управляет фокусом сущностей (IFocusable) и динамически вычисляет CursorMode.
    public class SessionGameplayState : SessionStateBase, IDisposable
    {
        public override string Name => "Gameplay";
        public override bool IsGameplay => true;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Gameplay;
        public override bool ResetMusicOnEnter => false;
        public override ITimeState TimeState => TimeStateFactory.Normal();

        private readonly ReactiveProperty<IFocusable> _currentFocus = new();
        private readonly ReactiveProperty<CursorStateMode> _focusCursorMode = new(CursorStateMode.Locked);
        private IDisposable _focusCursorSubscription;

        private Action<bool> _onKeyDown;

        public IReadOnlyReactiveProperty<IFocusable> CurrentFocus => _currentFocus;
        public IReadOnlyReactiveProperty<CursorStateMode> FocusCursorMode => _focusCursorMode;

        public override CursorStateMode CursorMode => _focusCursorMode.Value;

        public SessionGameplayState(SessionStateManager sm = null) : base(sm)
        {
            RegisterEvent(SessionEvent.Pause, () => SM?.PauseScreen);
            RegisterEvent(SessionEvent.StartEntitiesSync, () => SM?.EntitiesSync);
        }

        public override void Enter()
        {
            base.Enter();

            _onKeyDown = _ => SM?.Enqueue(SessionEvent.Pause);
            var button = GlobalGame.InputController?.GetButton(InputLayer.Inputs.E);
            if (button != null)
            {
                button.OnDown += _onKeyDown;
            }
        }

        public override void Exit()
        {
            if (_onKeyDown != null)
            {
                var button = GlobalGame.InputController?.GetButton(InputLayer.Inputs.E);
                if (button != null)
                {
                    button.OnDown -= _onKeyDown;
                }
                _onKeyDown = null;
            }

            base.Exit();
        }

        public void SetFocus(IFocusable focus)
        {
            if (_currentFocus.Value == focus) return;

            _focusCursorSubscription?.Dispose();
            _focusCursorSubscription = null;

            _currentFocus.Value?.LoseFocus();
            _currentFocus.Value = focus;

            if (focus != null && focus.CursorMode != null)
            {
                _focusCursorMode.Value = focus.CursorMode.Value;
                _focusCursorSubscription = focus.CursorMode.Subscribe(mode => _focusCursorMode.Value = mode);
            }
            else
            {
                _focusCursorMode.Value = CursorStateMode.Locked;
            }
        }

        public void Dispose()
        {
            if (_onKeyDown != null)
            {
                var button = GlobalGame.InputController?.GetButton(InputLayer.Inputs.E);
                if (button != null)
                {
                    button.OnDown -= _onKeyDown;
                }
                _onKeyDown = null;
            }

            _focusCursorSubscription?.Dispose();
            _currentFocus.Dispose();
            _focusCursorMode.Dispose();
        }
    }
}
