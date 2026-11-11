// Константы идентификаторов стандартных подсостояний паузы.
// Роль в проекте: предоставляет строковые ключи для регистрации и переключения оверлеев паузы.

namespace Game.Session.Pause
{
    // Идентификаторы типовых подсостояний паузы сессии (меню, консоль, настройки, сейвы).
    public static class PauseSubStateIds
    {
        // Главное меню паузы сессии (Продолжить, Сохранить, Выйти).
        public const string PauseMenu = "PauseMenu";
        // Консоль разработчика.
        public const string Console = "Console";
        // Экран настроек игры в паузе.
        public const string Settings = "Settings";
        // Меню сохранений и загрузки сейвов.
        public const string Saves = "Saves";
    }
}
