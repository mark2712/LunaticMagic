// Состояния (классы-команды) и фабрика подсистемы времени.
// Роль в проекте: инкапсулируют поведение физики Unity и продвижение календаря для 3 режимов (Пауза, Норма, Перемотка).

namespace LocalTime
{
    // ─── Контракт состояния времени (класс-команда) ───
    public interface ITimeState
    {
        float Scale { get; }

        // как сейчас вычисляется физика: в FixedUpdate или Update
        // false = FixedUpdate (физика активна), true = Update (физика замерла, календарь летит в Update)
        bool UpdateMode { get; }
    }

    // ─── 1. Состояние Паузы (мир и календарь наглухо заморожены) ───
    public sealed class PauseTimeState : ITimeState
    {
        public float Scale => 0f;
        public bool UpdateMode => false;
    }

    // ─── 2. Нормальное состояние симуляции мира (физика активна, тикает в FixedUpdate) ───
    public sealed class NormalTimeState : ITimeState
    {
        private readonly float _scale;

        public float Scale => _scale;
        public bool UpdateMode => false;

        public NormalTimeState(float scale = 1f)
        {
            _scale = scale;
        }
    }

    // ─── 3. Состояние Перемотки мира (физика замерла, календарь летит в Update) ───
    public sealed class FastForwardTimeState : ITimeState
    {
        private readonly float _gameTimeScale;

        public float Scale => _gameTimeScale;
        public bool UpdateMode => true;  // физика не активна, календарь тикает в Update

        public FastForwardTimeState(float gameTimeScale)
        {
            _gameTimeScale = gameTimeScale;
        }
    }
}
