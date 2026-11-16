using System;
using Game.Session.Pause;
using UniRx;
using UnityEngine;

// Менеджер состояний игровой сессии.
// Принимает реактивные флаги сессии, вычисляет актуальный SessionState через SessionPauseComputed
// и управляет жизненным циклом стейтов (Enter/Exit через GlobalGame.Session).
public class SessionStateManager : IDisposable
{
    private readonly ReactiveProperty<bool> _isSessionInitLoading;
    private readonly ReactiveProperty<bool> _isEntitiesSync;
    private readonly ReactiveProperty<bool> _isGameplay;

    private readonly ReactiveProperty<SessionState> _currentState;
    public IReadOnlyReactiveProperty<SessionState> CurrentState => _currentState;

    public IReadOnlyReactiveProperty<bool> IsSessionInitLoading => _isSessionInitLoading;
    public IReadOnlyReactiveProperty<bool> IsEntitiesSync => _isEntitiesSync;
    public IReadOnlyReactiveProperty<bool> IsGameplay => _isGameplay;

    private readonly CompositeDisposable _disposables = new();
    private int _batchDepth = 0;

    public SessionStateManager(
        ReactiveProperty<bool> isSessionInitLoading,
        ReactiveProperty<bool> isEntitiesSync,
        ReactiveProperty<bool> isGameplay)
    {
        _isSessionInitLoading = isSessionInitLoading;
        _isEntitiesSync = isEntitiesSync;
        _isGameplay = isGameplay;

        // Вычисляем начальное состояние на основе переданных флагов
        SessionState initialState = SessionPauseComputed.Computed(
            _isSessionInitLoading.Value,
            _isEntitiesSync.Value,
            _isGameplay.Value);

        _currentState = new ReactiveProperty<SessionState>(initialState);
        initialState.Enter();
        Debug.Log($"[SessionStateManager] State changed to: {initialState.Name}");

        // Реактивно отслеживаем изменение любого из флагов и переключаем стейт
        Observable.CombineLatest(
            _isSessionInitLoading,
            _isEntitiesSync,
            _isGameplay,
            (init, sync, gameplay) => SessionPauseComputed.Computed(init, sync, gameplay))
            .DistinctUntilChanged()
            .Subscribe(nextState =>
            {
                if (_batchDepth > 0) return; // Во время пакета пропускаем промежуточный дребезг

                ApplyState(nextState);
            })
            .AddTo(_disposables);
    }

    /// <summary>
    /// Пакетное изменение нескольких флагов (аналог runInAction / batch в MobX).
    /// Вычисляет стейт и вызывает Exit/Enter ровно один раз по завершении блока action.
    /// </summary>
    public void Batch(Action action)
    {
        _batchDepth++;
        try
        {
            action?.Invoke();
        }
        finally
        {
            _batchDepth--;
            if (_batchDepth <= 0)
            {
                _batchDepth = 0;
                Recompute();
            }
        }
    }
    // так используется Batch
    // SessionStateManager.Batch(() =>
    // {
    //     IsSessionInitLoading.Value = false;
    //     IsGameplay.Value = true;
    // });

    /// <summary>
    /// Пересчитать стейт на основе текущих значений флагов и применить его.
    /// </summary>
    public void Recompute()
    {
        SessionState computedState = SessionPauseComputed.Computed(
            _isSessionInitLoading.Value,
            _isEntitiesSync.Value,
            _isGameplay.Value);

        ApplyState(computedState);
    }

    private void ApplyState(SessionState nextState)
    {
        if (_currentState.Value != nextState)
        {
            _currentState.Value?.Exit();
            _currentState.Value = nextState;
            nextState.Enter();
            Debug.Log($"[SessionStateManager] State changed to: {nextState.Name}");
        }
    }

    public void EnterGameplay()
    {
        Batch(() =>
        {
            _isSessionInitLoading.Value = false;
            _isEntitiesSync.Value = false;
            _isGameplay.Value = true;
        });
    }

    public void SetGameplay(bool value) => _isGameplay.Value = value;
    public void SetEntitiesSync(bool value) => _isEntitiesSync.Value = value;
    public void SetSessionInitLoading(bool value) => _isSessionInitLoading.Value = value;

    public void Dispose()
    {
        _currentState.Value?.Exit();
        _disposables.Dispose();
    }
}
