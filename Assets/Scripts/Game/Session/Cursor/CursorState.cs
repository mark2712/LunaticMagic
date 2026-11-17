using System;
using Game.Session.Pause;
using UniRx;
using UnityEngine;

// Тип состояния курсора в сессии
public enum CursorStateMode
{
    Locked,
    Visible
}

// Конфигурация/пресет состояния курсора
public readonly struct CursorStateConfig
{
    public CursorStateMode Mode { get; }
    public CursorLockMode LockMode { get; }
    public bool IsVisible { get; }

    public static readonly CursorStateConfig Locked = new(CursorStateMode.Locked, CursorLockMode.Locked, false);
    public static readonly CursorStateConfig Visible = new(CursorStateMode.Visible, CursorLockMode.None, true);

    public CursorStateConfig(CursorStateMode mode, CursorLockMode lockMode, bool isVisible)
    {
        Mode = mode;
        LockMode = lockMode;
        IsVisible = isVisible;
    }

    public void Apply()
    {
        Cursor.lockState = LockMode;
        Cursor.visible = IsVisible;
    }

    public override string ToString() => $"[CursorStateConfig {Mode}: Lock={LockMode}, Visible={IsVisible}]";
}

public interface ICursorState : IDisposable
{
    IReadOnlyReactiveProperty<CursorStateMode> CurrentMode { get; }
    void Apply(CursorStateMode mode);
}

// Контроллер состояния курсора в сессии.
// Реактивно вычисляет стейт курсора через CursorComputed на основе 3 источников:
// 1. GlobalGame.UIGlobalState.IsAnyOpen
// 2. SessionStateManager.CurrentState
// 3. SessionStates.Gameplay.FocusCursorMode
public class CursorState : ICursorState
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ReactiveProperty<CursorStateMode> _currentMode = new(CursorStateMode.Visible);

    public IReadOnlyReactiveProperty<CursorStateMode> CurrentMode => _currentMode;

    public CursorState(SessionStateManager sessionStateManager)
        : this(sessionStateManager.CurrentState)
    {
    }

    public CursorState(IReadOnlyReactiveProperty<SessionState> sessionState)
    {
        Observable.CombineLatest(
            GlobalGame.UIGlobalState.IsAnyOpen,
            sessionState,
            SessionStates.Gameplay.FocusCursorMode,
            (isGlobalOpen, state, focusMode) => CursorComputed.Computed(isGlobalOpen, state, focusMode))
            .DistinctUntilChanged()
            .Subscribe(Apply)
            .AddTo(_disposables);
    }

    public void Apply(CursorStateMode mode)
    {
        _currentMode.Value = mode;
        switch (mode)
        {
            case CursorStateMode.Locked:
                CursorStateConfig.Locked.Apply();
                break;
            case CursorStateMode.Visible:
                CursorStateConfig.Visible.Apply();
                break;
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _currentMode.Dispose();
        CursorStateConfig.Visible.Apply();
    }
}
