namespace Entities.Locations
{
    /// <summary>
    /// Строковые идентификаторы локаций игрового мира.
    /// Каждая локация обладает собственным началом координат (0, 0, 0),
    /// что решает проблему потери точности float на больших дистанциях.
    /// </summary>
    public static class LocationIds
    {
        /// <summary>Специальная локация для сцены главного меню.</summary>
        public const string MainMenu = "MainMenu";

        /// <summary>Основная тестовая/дебажная игровая локация.</summary>
        public const string Debug1 = "Debug1";

        /// <summary>Локация по умолчанию для новых сущностей и инициализации сессии.</summary>
        public const string Default = Debug1;

        /// <summary>Проверка валидности идентификатора локации.</summary>
        public static bool IsValid(string locationId)
        {
            return locationId is MainMenu or Debug1;
        }
    }
}
