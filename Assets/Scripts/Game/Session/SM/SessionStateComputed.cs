namespace SessionSM
{
    // Чистый вычислитель актуального состояния сессии на основе флагов
    public static class SessionPauseComputed
    {
        public static SessionStateBase Computed(bool isSessionInitLoading, bool isEntitiesSync, bool isGameplay)
        {
            // 1. Первичная загрузка сессии: наивысший приоритет при старте мира.
            // Показывает экран InitialLoading с прогресс-баром.
            if (isSessionInitLoading)
            {
                return SessionStates.InitialLoading;
            }

            // 2. Синхронизация сущностей (стриминг чанков, блэкаут при телепорте/переходе)
            if (isEntitiesSync)
            {
                return SessionStates.EntitiesSync;
            }

            // 3. Экран меню паузы (Esc)
            if (!isGameplay)
            {
                return SessionStates.PauseScreen;
            }

            // 4. Активный геймплей
            return SessionStates.Gameplay;
        }

        public static SessionStateBase Computed(SessionStateManager sm, bool isSessionInitLoading, bool isEntitiesSync, bool isGameplay)
        {
            if (sm == null) return Computed(isSessionInitLoading, isEntitiesSync, isGameplay);

            if (isSessionInitLoading) return sm.InitialLoading;
            if (isEntitiesSync) return sm.EntitiesSync;
            if (!isGameplay) return sm.PauseScreen;
            return sm.Gameplay;
        }
    }
}
