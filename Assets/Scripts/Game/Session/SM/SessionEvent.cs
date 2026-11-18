namespace SessionSM
{
    // События жизненного цикла игровой сессии
    public enum SessionEvent
    {
        // Первичная загрузка и распаковка сессии завершена
        InitialLoadingFinished,

        // Запуск синхронизации мира / сущностей / стриминга чанков
        StartEntitiesSync,

        // Завершение синхронизации сущностей -> переход в геймплей
        FinishEntitiesSync,

        // Явный переход в паузу
        Pause,

        // Явный выход из паузы в геймплей
        Resume,

        // Перерасчет состояния через Computed (при изменении флагов)
        Recompute
    }
}
