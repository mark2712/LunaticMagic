using System;
using UniRx;

namespace Entities
{
    // Контракт сущности или компонента, способного принимать управление (фокус ввода и курсора).
    public interface IFocusable
    {
        // Реактивное свойство желаемого режима курсора.
        // Сущность меняет его на лету (например: вход в диалог -> Visible, выход -> Locked).
        IReadOnlyReactiveProperty<CursorStateMode> CursorMode { get; }

        // Сущность теряет управление (отписывается от ввода, сбрасывает скорости/анимации)
        void LoseFocus();
    }
}
