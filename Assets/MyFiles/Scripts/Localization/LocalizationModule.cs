using System;
using System.Collections.Generic;

public class LocalizationModule
{
    public Dictionary<string, string> Data; // < key, данные >
    public int RefCount;
    public DateTime LastUseTime; // Для умной очистки
    public void UpdateLastUseTime()
    {
        LastUseTime = DateTime.UtcNow;
    }
}
