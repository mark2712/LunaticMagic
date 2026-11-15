namespace Game.Session.Pause
{
    public class SessionPauseCmputed
    {
        public static void Computed()
        {
            var globalGame = GlobalGame.UIGlobalState.IsAnyOpen; // уже готов.
            bool isGameplay = false; // будет в сессии вместо IsPaused. Находится в GameSession
            bool isSessionInitLoading = false; // изначально в сессии при первичной загрузке, пока не завершена первая синхронизация сущностей. Находится в GameSession
            bool isEntitesSync = false; // активируется во время второй и тд синхронизации сущностей. Находится в GameSession
            // bool isPauseScreen = false; // переход возможен только в геймплее. Флага не будет, будет вычислятся как последний else в этом методе. Тоесть если нет GlobalGame.UIGlobalState.IsAnyOpen, isSessionInitLoading, isEntitesSync и isGameplay, то значит мы в паузе.

            // if (globalGame.Value)
            // {
            //     // глобальное состояние - источник GlobalGame.UIGlobalState.IsAnyOpen
            //     // курсор включен
            // }
            // else 
            if (isSessionInitLoading)
            {
                // начальная загрузка сессии - источник всегда сессия
                // курсор выключен
                // музыка загрузки
            }
            else if (isEntitesSync)
            {
                // синхронизация сущностей - источник система стриминга
                // курсор выключен
                // музыка геймплея не прерывается
            }
            else if (!isGameplay)
            {
                // показываем паузу
                // курсор включен
                // музыка паузы
            }
            else
            {
                // геймплей - состояние сессии
                // что с курсором решает сущность
                // музыка геймплея
                // ввод идет на сущность
            }
        }
    }
}