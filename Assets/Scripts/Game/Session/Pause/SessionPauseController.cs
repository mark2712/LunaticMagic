using System;
using UniRx;

namespace Game.Session.Pause
{
    // Реализация контроллера паузы сессии.
    public class SessionPauseController : ISessionPauseController
    {
        private readonly ReactiveProperty<bool> _isPaused = new(false);
        private readonly ReactiveProperty<PauseMode> _currentMode = new(PauseMode.PauseScreen);

        public IReadOnlyReactiveProperty<bool> IsPaused => _isPaused;
        public IReadOnlyReactiveProperty<PauseMode> CurrentMode => _currentMode;

        public void SetMode(PauseMode mode)
        {
            _currentMode.Value = mode;
            _isPaused.Value = true;
        }

        public void Resume()
        {
            _isPaused.Value = false;
        }

        public void Update()
        {
            // Кадровое обновление логики паузы (пока заглушка, чтобы не спамить исключениями в консоль)
        }

        public void Dispose()
        {
            _isPaused.Dispose();
            _currentMode.Dispose();
        }
    }
}
