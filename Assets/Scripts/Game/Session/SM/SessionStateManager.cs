using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace SessionSM
{
    // Машина состояний игровой сессии с очередью событий и отложенной обработкой в конце кадра.
    public class SessionStateManager : IDisposable
    {
        public SessionInitialLoadingState InitialLoading { get; }
        public SessionEntitiesSyncState EntitiesSync { get; }
        public SessionGameplayState Gameplay { get; }
        public SessionPauseScreenState PauseScreen { get; }

        private readonly Queue<SessionEvent> _eventQueue = new();
        private readonly ReactiveProperty<SessionStateBase> _currentState;
        public IReadOnlyReactiveProperty<SessionStateBase> CurrentState => _currentState;

        private readonly ReactiveProperty<bool> _isSessionInitLoading;
        private readonly ReactiveProperty<bool> _isEntitiesSync;
        private readonly ReactiveProperty<bool> _isGameplay;

        public IReadOnlyReactiveProperty<bool> IsSessionInitLoading => _isSessionInitLoading;
        public IReadOnlyReactiveProperty<bool> IsEntitiesSync => _isEntitiesSync;
        public IReadOnlyReactiveProperty<bool> IsGameplay => _isGameplay;

        private readonly CompositeDisposable _disposables = new();
        private bool _isSyncingFlags = false;

        public SessionStateManager()
            : this(new ReactiveProperty<bool>(true), new ReactiveProperty<bool>(false), new ReactiveProperty<bool>(false))
        {
        }

        public SessionStateManager(
            ReactiveProperty<bool> isSessionInitLoading,
            ReactiveProperty<bool> isEntitiesSync,
            ReactiveProperty<bool> isGameplay)
        {
            _isSessionInitLoading = isSessionInitLoading;
            _isEntitiesSync = isEntitiesSync;
            _isGameplay = isGameplay;

            InitialLoading = new SessionInitialLoadingState(this);
            EntitiesSync = new SessionEntitiesSyncState(this);
            Gameplay = new SessionGameplayState(this);
            PauseScreen = new SessionPauseScreenState(this);

            SessionStateBase initialState = SessionPauseComputed.Computed(
                this,
                _isSessionInitLoading.Value,
                _isEntitiesSync.Value,
                _isGameplay.Value);

            _currentState = new ReactiveProperty<SessionStateBase>(initialState);
            SyncFlags(initialState);
            initialState.Enter();
            Debug.Log($"[SessionStateManager] Начальное состояние: {initialState.Name}");

            // При изменении любого флага извне ставим в очередь запрос на перерасчет стейта
            Observable.Merge(
                _isSessionInitLoading.AsUnitObservable(),
                _isEntitiesSync.AsUnitObservable(),
                _isGameplay.AsUnitObservable())
                .Subscribe(_ =>
                {
                    if (_isSyncingFlags) return;
                    Enqueue(SessionEvent.Recompute);
                })
                .AddTo(_disposables);
        }

        /// <summary>
        /// Вычисляет актуальное состояние на основе текущих флагов сессии через SessionPauseComputed.
        /// </summary>
        public SessionStateBase Computed()
        {
            return SessionPauseComputed.Computed(
                this,
                _isSessionInitLoading != null && _isSessionInitLoading.Value,
                _isEntitiesSync != null && _isEntitiesSync.Value,
                _isGameplay != null && _isGameplay.Value);
        }

        /// <summary>
        /// Помещает событие в очередь. События обрабатываются в конце кадра (LateUpdate).
        /// </summary>
        public void Enqueue(SessionEvent evt)
        {
            _eventQueue.Enqueue(evt);
        }

        /// <summary>
        /// Вызывается в конце каждого кадра (из GameSession.LateUpdate()).
        /// Обрабатывает все накопившиеся события и переключает состояния при необходимости.
        /// </summary>
        public void LateUpdate()
        {
            ProcessQueue();
        }

        /// <summary>
        /// Обрабатывает очередь событий.
        /// </summary>
        public void ProcessQueue()
        {
            if (_eventQueue.Count == 0) return;

            while (_eventQueue.Count > 0)
            {
                var evt = _eventQueue.Dequeue();
                var nextState = _currentState.Value?.HandleEvent(evt);
                if (nextState != null && nextState != _currentState.Value)
                {
                    ChangeState(nextState);
                }
            }
        }

        /// <summary>
        /// Немедленная смена состояния.
        /// </summary>
        public void ChangeState(SessionStateBase nextState)
        {
            if (nextState == null || _currentState.Value == nextState) return;

            _currentState.Value?.Exit();
            _currentState.Value = nextState;
            SyncFlags(nextState);
            nextState.Enter();
            Debug.Log($"[SessionStateManager] State changed to: {nextState.Name}");
        }

        private void SyncFlags(SessionStateBase state)
        {
            if (state == null) return;

            _isSyncingFlags = true;
            try
            {
                if (_isSessionInitLoading != null)
                    _isSessionInitLoading.Value = state is SessionInitialLoadingState;

                if (_isEntitiesSync != null)
                    _isEntitiesSync.Value = state is SessionEntitiesSyncState;

                if (_isGameplay != null)
                    _isGameplay.Value = state.IsGameplay;
            }
            finally
            {
                _isSyncingFlags = false;
            }
        }

        public void Dispose()
        {
            _currentState.Value?.Exit();
            _disposables.Dispose();
            _currentState.Dispose();
            Gameplay?.Dispose();
        }
    }
}
