using System;
using System.Collections.Generic;
using Game.Session.Sound;
using LocalTime;
using UnityEngine;

namespace SessionSM
{
    // Базовый класс состояния игровой сессии
    public abstract class SessionStateBase
    {
        public SessionStateManager SM { get; internal set; }

        public abstract string Name { get; }
        public abstract bool IsGameplay { get; }
        public abstract CursorStateMode CursorMode { get; }

        public abstract SessionSoundSlot ActiveSoundSlot { get; }
        public abstract bool ResetMusicOnEnter { get; }
        public virtual ITimeState TimeState => TimeStateFactory.Pause();

        private readonly Dictionary<SessionEvent, Func<SessionStateBase>> _eventHandlers = new();

        protected SessionStateBase(SessionStateManager sm = null)
        {
            SM = sm;
            RegisterEvent(SessionEvent.Recompute, Computed);
        }

        /// <summary>
        /// Вычисляет актуальный стейт через SessionPauseComputed на основе флагов сессии.
        /// </summary>
        protected SessionStateBase Computed() => SM?.Computed();

        /// <summary>
        /// Регистрирует переход в состояние при возникновении события.
        /// </summary>
        protected void RegisterEvent(SessionEvent evt, Func<SessionStateBase> handler)
        {
            _eventHandlers[evt] = handler;
        }

        /// <summary>
        /// Регистрирует побочное действие без смены состояния при возникновении события.
        /// </summary>
        protected void RegisterEvent(SessionEvent evt, Action handler)
        {
            _eventHandlers[evt] = () =>
            {
                handler?.Invoke();
                return null;
            };
        }

        /// <summary>
        /// Обработка события текущим состоянием.
        /// Возвращает следующий стейт для перехода, либо null, если переход не требуется.
        /// </summary>
        public virtual SessionStateBase HandleEvent(SessionEvent evt)
        {
            if (_eventHandlers.TryGetValue(evt, out var handler))
            {
                return handler?.Invoke();
            }
            return null;
        }

        public virtual void Enter()
        {
            var session = GlobalGame.Session;
            session?.LocalTime?.SetState(TimeState);
            session?.SessionSoundController?.SetActiveSlot(ActiveSoundSlot, ResetMusicOnEnter);
        }

        public virtual void Exit() { }

        public virtual void Update() { }

        public override string ToString() => $"[SessionState: {Name}]";
    }

    // Кэшированные синглтон-экземпляры состояний
    public static class SessionStates
    {
        public static readonly SessionInitialLoadingState InitialLoading = new();
        public static readonly SessionEntitiesSyncState EntitiesSync = new();
        public static readonly SessionGameplayState Gameplay = new();
        public static readonly SessionPauseScreenState PauseScreen = new();
    }
}
