// Расчет внутриигрового календарного времени мира (секунды, минуты, часы, сутки).
// Роль в проекте: детерминированный учет времени суток в FixedUpdate/Update и сериализация в файл сохранения.

using System;
using UnityEngine;

namespace LocalTime
{
    // Модель календарного времени игрового мира.
    public sealed class LocalWorldGameTime
    {
        // Базовая скорость течения игрового времени (по умолчанию 60: 1 реальная секунда = 1 игровая минута).
        private float _timeScale = 60f;
        public float TimeScale
        {
            get => _timeScale;
            set => _timeScale = float.IsNaN(value) || float.IsInfinity(value) ? 60f : Mathf.Max(0f, value);
        }

        // Накопленные секунды игрового мира с момента старта сессии.
        public double WorldSeconds { get; private set; } = 0;
        public double WorldMinutes => WorldSeconds / 60.0;
        public double WorldHours => WorldSeconds / 3600.0;

        // Внутриигровой текущий день (начинается с 1).
        public int Day => (int)((long)WorldSeconds / 86400) + 1;
        // Текущий час суток (0..23).
        public int Hour => (int)(((long)WorldSeconds / 3600) % 24);
        // Текущая минута часа (0..59).
        public int Minute => (int)(((long)WorldSeconds / 60) % 60);
        // Текущая секунда минуты (0..59).
        public int Second => (int)((long)WorldSeconds % 60);

        // Продвигает календарное время мира на эффективный шаг дельты.
        public void Tick(float effectiveDeltaTime)
        {
            if (float.IsNaN(effectiveDeltaTime) || float.IsInfinity(effectiveDeltaTime) || effectiveDeltaTime <= 0f) return;
            WorldSeconds += (double)effectiveDeltaTime * _timeScale;
        }

        // Устанавливает абсолютное значение секунд мира (с защитой от отрицательных значений).
        public void SetTime(double seconds)
        {
            if (double.IsNaN(seconds) || double.IsInfinity(seconds)) return;
            WorldSeconds = Math.Max(0.0, seconds);
        }

        // Загружает сохраненное календарное время из структуры данных сохранения.
        public void Load(LocalGameTimeData data)
        {
            double seconds = data?.WorldSeconds ?? 0.0;
            if (double.IsNaN(seconds) || double.IsInfinity(seconds)) seconds = 0.0;
            WorldSeconds = Math.Max(0.0, seconds);
        }

        // Формирует структуру данных для сохранения времени в сейв сессии.
        public LocalGameTimeData Save()
        {
            return new LocalGameTimeData { WorldSeconds = WorldSeconds };
        }
    }
}
