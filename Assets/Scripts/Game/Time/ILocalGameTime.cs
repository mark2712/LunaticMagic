using System;
using UniRx;

namespace LocalTime
{
    // ─── Контракт фасада времени сессии ───
    public interface ILocalGameTime : IDisposable
    {
        // Базовый физический шаг Unity (50 Гц). Конфиг для всего проекта.
        public const float BaseFixedDeltaTime = 0.02f;

        // Текущее активное состояние времени сессии.
        IReadOnlyReactiveProperty<ITimeState> CurrentState { get; }

        // Устанавливает новое состояние времени (переключает режим движка и календаря).
        void SetState(ITimeState state);

        // Масштаб физического времени движка Unity (Time.timeScale).
        IReadOnlyReactiveProperty<float> PhysicsTimeScale { get; }

        // Масштаб скорости внутриигрового времени мира.
        IReadOnlyReactiveProperty<float> GameTimeScale { get; }

        // Дельта-шаг локального времени, прошедший за последний шаг (0 при паузе, масштабирован при геймплее/перемотке).
        // Используется системами баффов, DoT, регенерации и кулдаунов: Update(localTime.DeltaTime).
        float DeltaTime { get; }

        // Реактивный поток тиков времени мира. Эмиттит DeltaTime каждого шага.
        IObservable<float> OnTick { get; }

        // Продвигает физическое и календарное время мира в такт симуляции. Вызывается из FixedUpdate сессии.
        // Принимает Time.fixedDeltaTime (уже масштабированный контроллером через Time.timeScale).
        void FixedUpdate(float fixedDeltaTime);

        // Продвигает календарное время в режиме перемотки (когда физика замерла).
        // Принимает Time.unscaledDeltaTime. Расчет математически инвариантен к частоте кадров (не зависит от FPS).
        void Update(float unscaledDeltaTime);

        // Внутриигровой календарь
        double WorldSeconds { get; }
        double WorldMinutes { get; }
        double WorldHours { get; }
        int Day { get; }
        int Hour { get; }
        int Minute { get; }
        int Second { get; }

        // Сохранение и загрузка
        void Load(LocalGameTimeData data);
        LocalGameTimeData Save();
    }

    // Сериализуемая структура сохранения времени мира в файл сейва.
    [Serializable]
    public class LocalGameTimeData
    {
        public double WorldSeconds = 0;
    }
}
