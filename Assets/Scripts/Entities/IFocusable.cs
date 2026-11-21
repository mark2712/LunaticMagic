using System;
using UniRx;

namespace Entities
{
    /// <summary>
    /// Контракт сущности или компонента, способного принимать фокус управления сессии.
    /// Передает активность камеры, управление вводом и требуемый режим курсора.
    /// </summary>
    public interface IFocusable
    {
        /// <summary>Идентификатор сущности, владеющей фокусом.</summary>
        string EntityId { get; }

        /// <summary>Реактивное свойство желаемого режима курсора мыши для данной сущности.</summary>
        IReadOnlyReactiveProperty<CursorStateMode> CursorMode { get; }

        /// <summary>Получение фокуса: активация камеры сущности, подключение обработки ввода и HUD.</summary>
        void Focus();

        /// <summary>Потеря фокуса: деактивация камеры, скрытие HUD, сброс скоростей и осей ввода.</summary>
        void LoseFocus();
    }
}
