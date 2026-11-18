using SessionSM;

// Чистый вычислитель актуального состояния курсора на основе 3 параметров:
// 1. isGlobalUIOpen (UIGlobalState.IsAnyOpen: консоль ~, дебаг-меню F12)
// 2. sessionState (SessionStateManager.CurrentState: InitialLoading, EntitiesSync, PauseScreen, Gameplay)
// 3. focusCursorMode (CursorMode сущности в фокусе геймплея)
public static class CursorComputed
{
    public static CursorStateMode Computed(bool isGlobalUIOpen, SessionStateBase sessionState, CursorStateMode focusCursorMode)
    {
        // 1. Наивысший приоритет: открыт системный глобальный UI (консоль, дебаг-меню) -> курсор всегда виден
        if (isGlobalUIOpen)
        {
            return CursorStateMode.Visible;
        }

        // 2. Если сессия еще не определила состояние
        if (sessionState == null)
        {
            return CursorStateMode.Visible;
        }

        // 3. Активный геймплей: режим курсора определяет сущность в фокусе (инвентарь, диалог, свободный обзор)
        if (sessionState is SessionGameplayState)
        {
            return focusCursorMode;
        }

        // 4. Остальные состояния сессии:
        //    - InitialLoading -> Locked
        //    - EntitiesSync -> Locked
        //    - PauseScreen -> Visible
        return sessionState.CursorMode;
    }
}
