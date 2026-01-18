namespace States
{
    public enum StateEvent
    {
        MyEvent,
        JumpFinished,
        HitFinished,
        StartDialog,

        MoveChanged,
        GroundChanged,
        ShiftChanged,
        SneakChanged,

        HandsRopeChanged,
        LegsRopeChanged,

        GameOverChanged,

        Mouse1Performed, Mouse2Performed, Mouse3Performed,

        EscPerformed,
        ConsolePerformed,
        TabPerformed,
        SpacePerformed,
        ShiftPerformed, ShiftCanceled,
        CtrlPerformed, CtrlCanceled,
        AltPerformed, AltCanceled,

        KeyQ, KeyE, KeyR, KeyT, KeyI, KeyF, KeyZ, KeyX, KeyC,

        Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9, Num0,

        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    }
}