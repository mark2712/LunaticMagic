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
    CursorStateMode CurrentEffectiveMode { get; }
    CursorStateMode RequestedMode { get; }

    void Init(Func<CursorStateMode> sessionModeProvider, IObservable<Unit> stateChangedObservable = null);
    void SetState(CursorStateMode mode);
    void SetState(CursorStateConfig config);
    void Refresh();
}

// Контроллер состояния курсора в сессии.
// Учитывает приоритет глобального системного UI (UIGlobalState.IsAnyOpen - консоль, дебаг-меню):
// - Если открыт системный UI -> курсор принудительно виден и разблокирован (Visible).
// - Если системный UI закрыт -> опрашивает актуальный стейт сессии (провайдер) или запрошенный режим.
public class CursorState : ICursorState
{
    private readonly CompositeDisposable _disposables = new();
    private Func<CursorStateMode> _sessionModeProvider;
    private CursorStateMode _requestedMode = CursorStateMode.Visible;

    public CursorStateMode CurrentEffectiveMode { get; private set; } = CursorStateMode.Visible;
    public CursorStateMode RequestedMode => _requestedMode;

    public CursorState()
    {
        // Подписываемся на системный UI оверлей (Console ~, DebugMenu F12)
        if (GlobalGame.UIGlobalState?.IsAnyOpen != null)
        {
            GlobalGame.UIGlobalState.IsAnyOpen
                .Subscribe(_ => Refresh())
                .AddTo(_disposables);
        }

        // Подписываемся на динамическую смену курсора сущности в фокусе геймплея (диалоги, инвентарь)
        SessionStates.Gameplay.OnFocusCursorModeChanged += Refresh;
    }

    /// <summary>
    /// Инициализирует опрос актуального стейта сессии и подписку на изменение состояния.
    /// </summary>
    public void Init(Func<CursorStateMode> sessionModeProvider, IObservable<Unit> stateChangedObservable = null)
    {
        _sessionModeProvider = sessionModeProvider;
        if (stateChangedObservable != null)
        {
            stateChangedObservable
                .Subscribe(_ => Refresh())
                .AddTo(_disposables);
        }
        Refresh();
    }

    public void SetState(CursorStateMode mode)
    {
        _requestedMode = mode;
        Refresh();
    }

    public void SetState(CursorStateConfig config)
    {
        SetState(config.Mode);
    }

    public void Refresh()
    {
        // 1. Наивысший приоритет: системный оверлей (консоль или дебаг меню)
        if (GlobalGame.UIGlobalState?.IsAnyOpen != null && GlobalGame.UIGlobalState.IsAnyOpen.Value)
        {
            Apply(CursorStateConfig.Visible);
            return;
        }

        // 2. Опрашиваем провайдер актуального стейта сессии (если задан), иначе берем _requestedMode
        CursorStateMode targetMode = _sessionModeProvider != null
            ? _sessionModeProvider()
            : _requestedMode;

        switch (targetMode)
        {
            case CursorStateMode.Locked:
                Apply(CursorStateConfig.Locked);
                break;
            case CursorStateMode.Visible:
                Apply(CursorStateConfig.Visible);
                break;
        }
    }

    private void Apply(CursorStateConfig config)
    {
        CurrentEffectiveMode = config.Mode;
        config.Apply();
    }

    public void Dispose()
    {
        SessionStates.Gameplay.OnFocusCursorModeChanged -= Refresh;
        _disposables.Dispose();

        // При уничтожении освобождаем курсор для безопасности
        Apply(CursorStateConfig.Visible);
    }
}
