// Фасад подсистемы времени для игровой сессии GameSession.
// Роль в проекте: управляет физикой движка Unity (Time.timeScale, Time.fixedDeltaTime)
// и календарём мира (LocalWorldGameTime) под управлением стейт-машины ITimeState.

using System;
using UniRx;
using UnityEngine;

namespace LocalTime
{
    // Реализация фасада времени сессии.
    public sealed class LocalGameTime : ILocalGameTime
    {
        // Минимальный порог fixedDeltaTime для предотвращения зависания PhysX при экстремальном замедлении.
        private const float MinFixedDeltaTime = 0.0005f;
        // Максимальный предел масштаба движка Unity для исключения Physics Death Spiral.
        private const float MaxEngineTimeScale = 3.0f;

        private readonly LocalWorldGameTime _worldTime = new();

        private readonly ReactiveProperty<ITimeState> _currentState = new(TimeStateFactory.Pause());
        private readonly ReactiveProperty<float> _physicsTimeScale = new(0f);
        private readonly ReactiveProperty<float> _gameTimeScale = new(0f);
        private readonly Subject<float> _onTick = new();

        private float _deltaTime = 0f;
        private bool _isDisposed;

        public LocalGameTime()
        {
            SetState(TimeStateFactory.Pause());
        }

        // Текущее активное состояние времени сессии.
        public IReadOnlyReactiveProperty<ITimeState> CurrentState => _currentState;

        // Реактивное свойство текущего масштаба времени движка Unity (Time.timeScale).
        public IReadOnlyReactiveProperty<float> PhysicsTimeScale => _physicsTimeScale;

        // Реактивное свойство масштаба скорости внутриигрового времени.
        public IReadOnlyReactiveProperty<float> GameTimeScale => _gameTimeScale;

        // Эффективная дельта локального времени за последний шаг (для систем баффов, DoT, регенерации).
        public float DeltaTime => _deltaTime;

        // Реактивный поток тиков времени.
        public IObservable<float> OnTick => _onTick;

        // Внутриигровой календарь мира.
        public double WorldSeconds => _worldTime.WorldSeconds;
        public double WorldMinutes => _worldTime.WorldMinutes;
        public double WorldHours => _worldTime.WorldHours;

        public int Day => _worldTime.Day;
        public int Hour => _worldTime.Hour;
        public int Minute => _worldTime.Minute;
        public int Second => _worldTime.Second;

        // Применяет новое состояние времени, настраивая масштаб движка и скорость календаря.
        public void SetState(ITimeState state)
        {
            if (_isDisposed) return;
            state ??= TimeStateFactory.Pause();
            _currentState.Value = state;

            // UpdateMode = true (FastForward): физика замерла, календарь летит в Update.
            // UpdateMode = false (Normal/Pause): физика активна со Scale, календарь тикает в FixedUpdate.
            float physicsScale = state.UpdateMode ? 0f : state.Scale;
            ApplyEngineTimeScale(physicsScale);
            _physicsTimeScale.Value = physicsScale;
            _gameTimeScale.Value = state.Scale;
            _deltaTime = 0f;
        }

        // Продвигает физическое и календарное время мира. Вызывается строго из FixedUpdate сессии.
        // Принимает Time.fixedDeltaTime (уже масштабированный через Time.timeScale).
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (_isDisposed) return;

            var state = _currentState.Value;
            if (state == null || state.UpdateMode || state.Scale <= 0f) return;

            // fixedDeltaTime уже масштабирован (Time.fixedDeltaTime = Base * scale).
            _worldTime.Tick(fixedDeltaTime);
            _deltaTime = fixedDeltaTime;
            _onTick.OnNext(fixedDeltaTime);
        }

        // Продвигает календарное время в режиме перемотки (когда физика на нуле). Вызывается из Update сессии.
        // Принимает Time.unscaledDeltaTime. Расчет математически инвариантен к частоте кадров (не зависит от FPS).
        public void Update(float unscaledDeltaTime)
        {
            if (_isDisposed) return;

            var state = _currentState.Value;
            if (state == null || !state.UpdateMode || state.Scale <= 0f) return;

            float dt = unscaledDeltaTime * state.Scale;
            _worldTime.Tick(dt);
            _deltaTime = dt;
            _onTick.OnNext(dt);
        }

        // Загружает данные времени из файла сохранения.
        public void Load(LocalGameTimeData data)
        {
            _worldTime.Load(data);
        }

        // Сохраняет текущие секунды мира для структуры сохранения.
        public LocalGameTimeData Save()
        {
            return _worldTime.Save();
        }

        // Задает масштаб времени движка и адаптирует fixedDeltaTime с защитой от сбоев физики.
        private void ApplyEngineTimeScale(float scale)
        {
            if (float.IsNaN(scale) || float.IsInfinity(scale)) scale = 1f;
            scale = Mathf.Clamp(scale, 0f, MaxEngineTimeScale);

            bool frozen = scale <= 0.0001f;

            Time.timeScale = frozen ? 0f : scale;
            Time.fixedDeltaTime = frozen
                ? ILocalGameTime.BaseFixedDeltaTime
                : Mathf.Clamp(ILocalGameTime.BaseFixedDeltaTime * scale, MinFixedDeltaTime, ILocalGameTime.BaseFixedDeltaTime * MaxEngineTimeScale);
        }

        // Освобождает реактивные ресурсы и сбрасывает движок Unity в дефолтное состояние.
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _onTick.OnCompleted();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = ILocalGameTime.BaseFixedDeltaTime;
            _currentState.Dispose();
            _physicsTimeScale.Dispose();
            _gameTimeScale.Dispose();
            _onTick.Dispose();
        }
    }
}
