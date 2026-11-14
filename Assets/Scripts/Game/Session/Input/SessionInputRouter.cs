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
    void Init(IReadOnlyReactiveProperty<bool> isPaused);
}

// Маршрутизатор / шлюз пользовательского ввода для текущей игровой сессии.
// Работает как фильтрующая прокладка над GlobalGame.InputController:
// - Если игра НЕ на паузе: транслирует события ввода сущностям сессии.
// - Если игра на паузе: блокирует игровой ввод и сбрасывает значения в дефолтные (предотвращая залипание клавиш).
public class SessionInputRouter : ISessionInputRouter
{
    private readonly IInputController _inputController = GlobalGame.InputController;
    private readonly List<Action> _unsubscribers = new();
    private readonly Dictionary<Inputs, AxisInput<bool>> _buttons = new();
    private IDisposable _pauseSubscription;
    private IReadOnlyReactiveProperty<bool> _isPaused;

    public AxisInput<Vector2> Move { get; } = new(Vector2.zero);
    public AxisInput<Vector2> Look { get; } = new(Vector2.zero);
    public AxisInput<float> Scroll { get; } = new(0f);

    public AxisInput<bool> GetButton(Inputs input)
    {
        return _buttons.TryGetValue(input, out var button) ? button : null;
    }

    private bool IsPaused => _isPaused != null && _isPaused.Value;

    public SessionInputRouter()
    {
        // Инициализируем оси кнопок для всех поддерживаемых клавиш ввода
        foreach (Inputs input in Enum.GetValues(typeof(Inputs)))
        {
            if (input is Inputs.Move or Inputs.Look or Inputs.Scroll)
                continue;

            _buttons[input] = new AxisInput<bool>(false);
        }
    }

    public void Init(IReadOnlyReactiveProperty<bool> isPaused)
    {
        Dispose();

        _isPaused = isPaused;

        if (_inputController == null)
            return;

        // Отслеживаем переход паузы: при входе сбрасываем ввод, при выходе восстанавливаем
        if (_isPaused != null)
        {
            _pauseSubscription = _isPaused.Subscribe(OnPauseChanged);
        }

        SubscribeAxes();
        SubscribeButtons();
    }

    private void SubscribeAxes()
    {
        // Move (Vector2)
        Action<Vector2> onMove = v =>
        {
            if (!IsPaused)
                Move.SetValue(v);
        };
        _inputController.Move.OnChanged += onMove;
        _unsubscribers.Add(() => _inputController.Move.OnChanged -= onMove);

        // Look (Vector2)
        Action<Vector2> onLook = v =>
        {
            if (!IsPaused)
                Look.SetValue(v);
        };
        _inputController.Look.OnChanged += onLook;
        _unsubscribers.Add(() => _inputController.Look.OnChanged -= onLook);

        // Scroll (float)
        Action<float> onScroll = v =>
        {
            if (!IsPaused)
                Scroll.SetValue(v);
        };
        _inputController.Scroll.OnChanged += onScroll;
        _unsubscribers.Add(() => _inputController.Scroll.OnChanged -= onScroll);
    }

    private void SubscribeButtons()
    {
        foreach (Inputs input in Enum.GetValues(typeof(Inputs)))
        {
            if (input is Inputs.Move or Inputs.Look or Inputs.Scroll)
                continue;

            var globalButton = _inputController.GetButton(input);
            if (globalButton == null) continue;

            Inputs captured = input;
            Action<bool> onButtonChanged = v =>
            {
                if (!IsPaused)
                    _buttons[captured].SetValue(v);
            };

            globalButton.OnChanged += onButtonChanged;
            _unsubscribers.Add(() => globalButton.OnChanged -= onButtonChanged);
        }
    }

    private void OnPauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            // Сбрасываем весь игровой ввод в дефолтные значения (Vector2.zero, 0f, false)
            // Это вызовет OnUp на зажатых клавишах и остановит перемещение сущностей
            ResetToDefault();
        }
        else
        {
            // При снятии с паузы синхронизируем с актуальным физическим вводом
            SyncWithGlobalInput();
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

    public void Dispose()
    {
        _pauseSubscription?.Dispose();
        _pauseSubscription = null;

        foreach (var unsub in _unsubscribers)
        {
            unsub();
        }
        _unsubscribers.Clear();

        ResetToDefault();
        _isPaused = null;
    }
}
