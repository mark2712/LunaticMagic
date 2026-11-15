using System;
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

    void SetState(CursorStateMode mode);
    void SetState(CursorStateConfig config);
    void Refresh();
}

// Контроллер состояния курсора в сессии.
// Учитывает приоритет глобального системного UI (UIGlobalState.IsAnyOpen - консоль, дебаг-меню):
// - Если открыт системный UI -> курсор принудительно виден и разблокирован (Visible).
// - Если системный UI закрыт -> применяется режим, запрошенный сущностью или состоянием паузы.
public class CursorState : ICursorState
{
    private readonly CompositeDisposable _disposables = new();
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

        // 2. Стандартный режим сессии (запрошенный сущностью в фокусе или стейтом паузы)
        switch (_requestedMode)
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
        _disposables.Dispose();

        // При уничтожении освобождаем курсор для безопасности
        Apply(CursorStateConfig.Visible);
    }
}
