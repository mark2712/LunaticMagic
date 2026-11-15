namespace Entities
{
    public interface IFocusable
    {
        // Сущность получает управление и роутер ввода сессии
        void Focus(ISessionInputRouter inputRouter);

        // Сущность теряет управление (отписывается от роутера, обнуляет локальные скорости/анимации)
        void LoseFocus();
    }
}