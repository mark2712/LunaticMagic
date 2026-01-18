namespace InputLayer
{
    /// <summary>
    /// Логические кнопки ввода.
    /// НЕ равны StateEvent и не обязаны 1:1 соответствовать реальным клавишам.
    /// Это "физический ввод", а не "геймплейное намерение".
    /// </summary>
    public enum Inputs
    {
        Move, Look, Scroll,
        Mouse1, Mouse2, Mouse3,
        Shift, Ctrl, Alt, Space,
        Q, E, R, T, I, F, Z, X, C,
        Tab, Esc, Console,
        Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9, Num0,
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    }
}