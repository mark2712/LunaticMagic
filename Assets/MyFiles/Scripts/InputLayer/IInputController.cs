using System;
using UnityEngine;

namespace InputLayer
{

    public interface IInputController
    {
        /// <summary>
        /// Unity InputActions (для enable/disable, rebinding и тд.)
        /// </summary>
        PlayerInputActions InputActions { get; }

        AxisInput<Vector2> Move { get; }
        AxisInput<Vector2> Look { get; }
        AxisInput<float> Scroll { get; }

        /// <summary>
        /// Кнопка по идентификатору физического ввода
        /// </summary>
        AxisInput<bool> GetButton(Inputs input);

        /// <summary>
        /// Обновление импульсных осей (например Scroll)
        /// </summary>
        void LateUpdate();
    }
}
