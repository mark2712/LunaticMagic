using System;
using UniRx;

namespace Game.Session.Pause
{
    // Контракт контроллера паузы сессии.
    // Управляет режимами паузы (FocusTransition, PauseScreen, InitialLoading) и флагом IsPaused.
    public interface ISessionPauseController : IDisposable
    {
        // Активна ли системная пауза в данный момент.
        IReadOnlyReactiveProperty<bool> IsPaused { get; }

        // Текущий режим паузы (актуален, когда IsPaused == true).
        IReadOnlyReactiveProperty<PauseMode> CurrentMode { get; }

        // Переводит сессию в паузу с указанным режимом.
        void SetMode(PauseMode mode);

        // Снимает сессию с паузы и возвращает в геймплей.
        void Resume();

        // Кадровое обновление логики активного режима паузы.
        void Update();
    }
}
