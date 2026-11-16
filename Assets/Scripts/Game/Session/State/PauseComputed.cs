namespace Game.Session.Pause
{
    // Чистый вычислитель актуального состояния сессии на основе флагов
    public static class SessionPauseComputed
    {
        public static SessionState Computed(bool isSessionInitLoading, bool isEntitiesSync, bool isGameplay)
        {
            if (isSessionInitLoading)
            {
                // Начальная загрузка сессии — источник всегда сессия
                // Курсор выключен, музыка загрузки
                return SessionStates.InitialLoading;
            }

            if (isEntitiesSync)
            {
                // Синхронизация сущностей — источник система стриминга
                // Курсор выключен, музыка геймплея не прерывается
                return SessionStates.EntitiesSync;
            }

            if (!isGameplay)
            {
                // Экран паузы
                // Курсор включен, музыка паузы
                return SessionStates.PauseScreen;
            }

            // Активный геймплей
            // Курсор решает сущность, музыка геймплея, ввод на сущность
            return SessionStates.Gameplay;
        }
    }
}
