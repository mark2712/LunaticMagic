using System;
using UnityEngine;


public sealed class LocalWorldGameTime
{
    public float TimeScale { get; set; } = 60f; // 1 реальная секунда = 1 минута игрового времени

    public double WorldSeconds { get; private set; } = 0;
    public double WorldMinutes => WorldSeconds / 60.0;
    public double WorldHours => WorldSeconds / 3600.0;

    public int Day => (int)(WorldSeconds / 86400) + 1;
    public int Hour => (int)(WorldSeconds / 3600) % 24;
    public int Minute => (int)(WorldSeconds / 60) % 60;

    public void Tick(float deltaTime)
    {
        WorldSeconds += deltaTime * TimeScale;
    }

    public void SetTime(double seconds)
    {
        WorldSeconds = Math.Max(0, seconds);
    }

    public void Load(LocalGameTimeData data)
    {
        WorldSeconds = data?.WorldSeconds ?? 0;
    }

    public LocalGameTimeData Save()
    {
        return new LocalGameTimeData { WorldSeconds = WorldSeconds };
    }
}

