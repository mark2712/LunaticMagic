// Реализация диспетчера подсостояний паузы сессии.
// Роль в проекте: управляет жизненным циклом активного подстейта паузы (меню, консоль) и безопасной очисткой ресурсов.

using System;
using UniRx;

namespace Game.Session.Pause
{
    // Контроллер подсостояний паузы. Обеспечивает смену оверлеев и их выгрузку при возобновлении игры.
    public class PauseController : IPauseController
    {
        private readonly ReactiveProperty<IPauseSubState> _currentSubState = new();
        public IReadOnlyReactiveProperty<IPauseSubState> CurrentSubState => _currentSubState;

        // Переключает активный подстейт с вызовом жизненного цикла Exit() у старого и Enter() у нового.
        public void ChangeSubState(IPauseSubState newSubState)
        {
            if (_currentSubState.Value == newSubState) return;

            IPauseSubState previous = _currentSubState.Value;
            _currentSubState.Value = newSubState;
            previous?.Exit();
            newSubState?.Enter();
        }

        // Обновляет активный подстейт в цикле Update сессии.
        public void Update()
        {
            _currentSubState.Value?.Update();
        }

        // Сбрасывает активный подстейт при выходе из паузы, закрывая оверлей.
        public void Reset()
        {
            if (_currentSubState.Value != null)
            {
                IPauseSubState previous = _currentSubState.Value;
                _currentSubState.Value = null;
                previous.Exit();
            }
        }

        // Освобождает ресурсы контроллера, вызывая Exit() и Dispose() у активного подстейта.
        public void Dispose()
        {
            if (_currentSubState.Value != null)
            {
                IPauseSubState previous = _currentSubState.Value;
                _currentSubState.Value = null;
                previous.Exit();
                previous.Dispose();
            }
            _currentSubState.Dispose();
        }
    }
}
