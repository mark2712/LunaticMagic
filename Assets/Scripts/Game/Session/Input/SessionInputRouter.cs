using System;
using System.Collections.Generic;
using InputLayer;
using UniRx;
using UnityEngine;

public interface ISessionInputRouter : IDisposable
{
    AxisInput<Vector2> Move { get; }
    AxisInput<Vector2> Look { get; }
    AxisInput<float> Scroll { get; }
    AxisInput<bool> GetButton(Inputs input);
}

// Базовый класс для маршрутизаторов ввода сессии.
// Автоматически управляет подписками на физический GlobalGame.InputController,
// фильтрует ввод через IsActiveEffective и реагирует на системный оверлей (Console ~, DebugMenu F12).
public abstract class SessionInputRouter : ISessionInputRouter
{
    private readonly IInputController _inputController = GlobalGame.InputController;
    private readonly List<Action> _unsubscribers = new();
    private readonly Dictionary<Inputs, AxisInput<bool>> _buttons = new();
    private readonly CompositeDisposable _disposables = new();

    public AxisInput<Vector2> Move { get; } = new(Vector2.zero);
    public AxisInput<Vector2> Look { get; } = new(Vector2.zero);
    public AxisInput<float> Scroll { get; } = new(0f);

    public AxisInput<bool> GetButton(Inputs input)
    {
        return _buttons.TryGetValue(input, out var button) ? button : null;
    }

    /// <summary>
    /// Локальное условие активности конкретного канала (например, IsGameplay для геймплея или !IsGameplay для паузы)
    /// </summary>
    protected abstract bool IsConditionMet { get; }

    /// <summary>
    /// Итоговая активность канала: локальное условие выполнено И глобальный системный UI (консоль/дебаг) закрыт
    /// </summary>
    public bool IsActiveEffective
    {
        get
        {
            if (!IsConditionMet)
                return false;

            if (GlobalGame.UIGlobalState?.IsAnyOpen != null && GlobalGame.UIGlobalState.IsAnyOpen.Value)
                return false;

            return true;
        }
    }

    protected SessionInputRouter()
    {
        // Инициализируем оси кнопок
        foreach (Inputs input in Enum.GetValues(typeof(Inputs)))
        {
            if (input is Inputs.Move or Inputs.Look or Inputs.Scroll)
                continue;

            _buttons[input] = new AxisInput<bool>(false);
        }

        // Подписываемся на системный UI (Console ~, DebugMenu F12)
        if (GlobalGame.UIGlobalState?.IsAnyOpen != null)
        {
            GlobalGame.UIGlobalState.IsAnyOpen
                .Subscribe(_ => OnStateChanged())
                .AddTo(_disposables);
        }
    }

    protected void BindCondition(IReadOnlyReactiveProperty<bool> condition)
    {
        condition?
            .Subscribe(_ => OnStateChanged())
            .AddTo(_disposables);

        SubscribeAxes();
        SubscribeButtons();
        OnStateChanged();
    }

    private void SubscribeAxes()
    {
        if (_inputController == null) return;

        Action<Vector2> onMove = v => { if (IsActiveEffective) Move.SetValue(v); };
        _inputController.Move.OnChanged += onMove;
        _unsubscribers.Add(() => _inputController.Move.OnChanged -= onMove);

        Action<Vector2> onLook = v => { if (IsActiveEffective) Look.SetValue(v); };
        _inputController.Look.OnChanged += onLook;
        _unsubscribers.Add(() => _inputController.Look.OnChanged -= onLook);

        Action<float> onScroll = v => { if (IsActiveEffective) Scroll.SetValue(v); };
        _inputController.Scroll.OnChanged += onScroll;
        _unsubscribers.Add(() => _inputController.Scroll.OnChanged -= onScroll);
    }

    private void SubscribeButtons()
    {
        if (_inputController == null) return;

        foreach (Inputs input in Enum.GetValues(typeof(Inputs)))
        {
            if (input is Inputs.Move or Inputs.Look or Inputs.Scroll)
                continue;

            var globalButton = _inputController.GetButton(input);
            if (globalButton == null) continue;

            Inputs captured = input;
            Action<bool> onButtonChanged = v =>
            {
                if (IsActiveEffective)
                    _buttons[captured].SetValue(v);
            };

            globalButton.OnChanged += onButtonChanged;
            _unsubscribers.Add(() => globalButton.OnChanged -= onButtonChanged);
        }
    }

    protected void OnStateChanged()
    {
        if (IsActiveEffective)
        {
            SyncWithGlobalInput();
        }
        else
        {
            ResetToDefault();
        }
    }

    private void ResetToDefault()
    {
        Move.SetValue(Vector2.zero);
        Look.SetValue(Vector2.zero);
        Scroll.SetValue(0f);

        foreach (var button in _buttons.Values)
        {
            button.SetValue(false);
        }
    }

    private void SyncWithGlobalInput()
    {
        if (_inputController == null) return;

        Move.SetValue(_inputController.Move.Value);
        Look.SetValue(_inputController.Look.Value);
        Scroll.SetValue(_inputController.Scroll.Value);

        foreach (var (input, button) in _buttons)
        {
            var globalButton = _inputController.GetButton(input);
            if (globalButton != null)
            {
                button.SetValue(globalButton.Value);
            }
        }
    }

    public virtual void Dispose()
    {
        _disposables.Dispose();

        foreach (var unsub in _unsubscribers)
        {
            unsub();
        }
        _unsubscribers.Clear();

        ResetToDefault();
    }
}

// Канал геймплейного ввода: активен, когда IsGameplay == true и системный оверлей закрыт.
// Предназначен для сущностей игрового мира (персонаж, турель, транспорт).
public class InputRouterGameplay : SessionInputRouter
{
    private IReadOnlyReactiveProperty<bool> _isGameplay;

    protected override bool IsConditionMet => _isGameplay != null && _isGameplay.Value;

    public void Init(IReadOnlyReactiveProperty<bool> isGameplay)
    {
        _isGameplay = isGameplay;
        BindCondition(isGameplay);
    }
}

// Канал ввода паузы: активен, когда IsGameplay == false и системный оверлей закрыт.
// Предназначен для UI экрана паузы (навигация, Esc для выхода из меню).
public class InputRouterPause : SessionInputRouter
{
    private IReadOnlyReactiveProperty<bool> _isGameplay;

    protected override bool IsConditionMet => _isGameplay != null && !_isGameplay.Value;

    public void Init(IReadOnlyReactiveProperty<bool> isGameplay)
    {
        _isGameplay = isGameplay;
        BindCondition(isGameplay);
    }
}
