namespace LocalTime
{
    // ─── Фабрика для получения готовых классов состояний времени ───
    public static class TimeStateFactory
    {
        private static readonly PauseTimeState _pause = new();
        private static readonly NormalTimeState _normalDefault = new(1f);

        // Возвращает состояние системной паузы (физика 0, календарь 0).
        public static ITimeState Pause() => _pause;

        // Возвращает нормальное состояние симуляции (физика и календарь идут синхронно).
        public static ITimeState Normal() => _normalDefault;
        public static ITimeState Normal(float scale) => new NormalTimeState(scale);

        // Возвращает состояние быстрой перемотки (физика 0, календарь быстро крутится в Update).
        public static ITimeState FastForward(float scale = 50f) => new FastForwardTimeState(scale);
    }
}