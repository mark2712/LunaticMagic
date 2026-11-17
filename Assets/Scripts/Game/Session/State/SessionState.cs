using System;
using Entities;
using Game.Session.Sound;
using LocalTime;
using UniRx;
using UnityEngine;

namespace Game.Session.Pause
{
    // Базовый класс состояния игровой сессии.
    public abstract class SessionState
    {
        public abstract string Name { get; }
        public abstract bool IsGameplay { get; }
        public abstract CursorStateMode CursorMode { get; }

        public abstract SessionSoundSlot ActiveSoundSlot { get; }
        public abstract bool ResetMusicOnEnter { get; }
        public virtual ITimeState TimeState => TimeStateFactory.Pause();

        public virtual void Enter()
        {
            var session = GlobalGame.Session;
            session?.LocalTime?.SetState(TimeState);
            session?.SessionSoundController?.SetActiveSlot(ActiveSoundSlot, ResetMusicOnEnter);
        }
        public virtual void Exit() { }

        public override string ToString() => $"[SessionState: {Name}]";
    }

    // 1. Начальная загрузка сессии (распаковка сейва, первичная инициализация мира)
    public class SessionStateInitialLoading : SessionState
    {
        public override string Name => "InitialLoading";
        public override bool IsGameplay => false;
        public override CursorStateMode CursorMode => CursorStateMode.Visible;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Loading;
        public override bool ResetMusicOnEnter => true;
    }

    // 2. Синхронизация сущностей (стриминг чанков, блэкаут при телепорте/переходе)
    public class SessionStateEntitiesSync : SessionState
    {
        public override string Name => "EntitiesSync";
        public override bool IsGameplay => false;
        public override CursorStateMode CursorMode => CursorStateMode.Locked;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Gameplay;
        public override bool ResetMusicOnEnter => false;
    }

    // 3. Экран меню паузы (Esc)
    public class SessionStatePauseScreen : SessionState
    {
        public override string Name => "PauseScreen";
        public override bool IsGameplay => false;
        public override CursorStateMode CursorMode => CursorStateMode.Visible;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Pause;
        public override bool ResetMusicOnEnter => true;
    }

    // 4. Активный игровой мир (геймплей)
    // Динамически опрашивает реактивный CursorMode у сущности в фокусе (IFocusable); если фокуса нет — отдаёт Locked.
    public class SessionStateGameplay : SessionState
    {
        public override string Name => "Gameplay";
        public override bool IsGameplay => true;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Gameplay;
        public override bool ResetMusicOnEnter => false;
        public override ITimeState TimeState => TimeStateFactory.Normal();

        private readonly ReactiveProperty<IFocusable> _currentFocus = new();
        private readonly ReactiveProperty<CursorStateMode> _focusCursorMode = new(CursorStateMode.Locked);
        private IDisposable _focusCursorSubscription;

        public IReadOnlyReactiveProperty<IFocusable> CurrentFocus => _currentFocus;
        public IReadOnlyReactiveProperty<CursorStateMode> FocusCursorMode => _focusCursorMode;

        public override CursorStateMode CursorMode => _focusCursorMode.Value;

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
    }

    // Кэшированные синглтон-экземпляры состояний
    public static class SessionStates
    {
        public static readonly SessionState InitialLoading = new SessionStateInitialLoading();
        public static readonly SessionState EntitiesSync = new SessionStateEntitiesSync();
        public static readonly SessionStatePauseScreen PauseScreen = new SessionStatePauseScreen();
        public static readonly SessionStateGameplay Gameplay = new SessionStateGameplay();
    }
}