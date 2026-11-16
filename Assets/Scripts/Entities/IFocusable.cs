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

        // Сущность получает управление и роутер геймплейного ввода сессии
        void Focus(ISessionInputRouter inputRouter);

        // Сущность теряет управление (отписывается от ввода, сбрасывает скорости/анимации)
        void LoseFocus();
    }

    // Базовый компонент/класс для сущностей с поддержкой фокуса ввода и динамического курсора.
    // Пример использования: персонаж в обычном состоянии держит Locked; при открытии диалога/инвентаря переключает в Visible.
    public class FocusableComponent : IFocusable, IDisposable
    {
        private readonly ReactiveProperty<CursorStateMode> _cursorMode;
        private readonly CompositeDisposable _disposables = new();

        public IReadOnlyReactiveProperty<CursorStateMode> CursorMode => _cursorMode;
        public bool HasFocus { get; private set; }

        public FocusableComponent(CursorStateMode initialMode = CursorStateMode.Locked)
        {
            _cursorMode = new ReactiveProperty<CursorStateMode>(initialMode);
        }

        public void SetCursorMode(CursorStateMode mode)
        {
            _cursorMode.Value = mode;
        }

        public virtual void Focus(ISessionInputRouter inputRouter)
        {
            HasFocus = true;
        }

        public virtual void LoseFocus()
        {
            HasFocus = false;
        }

        public virtual void Dispose()
        {
            _disposables.Dispose();
            _cursorMode.Dispose();
        }
    }
}
