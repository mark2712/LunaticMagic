// Контракт диспетчера подсостояний паузы сессии.
// Роль в проекте: предоставляет точку расширения для регистрации, переключения и обновления подстейтов паузы.

using System;
using UniRx;

namespace Game.Session.Pause
{
    // Диспетчер подсостояний паузы сессии. Управляет переключением активного оверлея во время системной паузы.
    public interface IPauseController : IDisposable
    {
        // Реактивное свойство текущего активного подсостояния паузы (null, если ни один оверлей не открыт).
        IReadOnlyReactiveProperty<IPauseSubState> CurrentSubState { get; }

        // Переключает активное подсостояние (вызывает Exit у предыдущего и Enter у нового).
        void ChangeSubState(IPauseSubState newSubState);

        // Кадровое обновление активного подсостояния паузы.
        void Update();

        // Сбрасывает активное подсостояние при выходе из паузы в геймплей.
        void Reset();
    }
}
