using System;

namespace InputLayer
{
    public interface IAxisInput<T>
    {
        /// <summary>
        /// Текущее значение ввода
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Дефолтное (неактивное) значение ввода
        /// </summary>
        T DefaultValue { get; }

        /// <summary>
        /// Значение изменилось
        /// </summary>
        event Action<T> OnChanged;

        /// <summary>
        /// Значение ввода стало НЕ дефолтным в этом кадре
        /// </summary>
        event Action<T> OnDown;

        /// <summary>
        /// Значение ввода стало дефолтным в этом кадре
        /// </summary>
        event Action<T> OnUp;
    }
}