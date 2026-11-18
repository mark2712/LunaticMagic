using System;
using Game.Session.Sound;
using LocalTime;
using UnityEngine;

namespace SessionSM
{
    // Меню паузы сессии
    public class SessionPauseScreenState : SessionStateBase
    {
        public override string Name => "PauseScreen";
        public override bool IsGameplay => false;
        public override CursorStateMode CursorMode => CursorStateMode.Visible;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Pause;
        public override bool ResetMusicOnEnter => true;
        public override ITimeState TimeState => TimeStateFactory.Pause();

        private Action<bool> _onKeyDown;

        public SessionPauseScreenState(SessionStateManager sm = null) : base(sm)
        {
            RegisterEvent(SessionEvent.Resume, () => SM?.Gameplay);
        }

        public override void Enter()
        {
            base.Enter();

            _onKeyDown = _ => SM?.Enqueue(SessionEvent.Resume);
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
    }
}
