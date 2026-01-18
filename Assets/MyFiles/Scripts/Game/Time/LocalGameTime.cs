using System;
using UniRx;
using UnityEngine;


public sealed class LocalGameTime
{
    private readonly GameTimeController _timeController = new();
    private readonly LocalWorldGameTime _worldTime = new();

    public IReadOnlyReactiveProperty<bool> IsPaused => _timeController.IsPaused;

    public int Day => _worldTime.Day;
    public int Hour => _worldTime.Hour;
    public int Minute => _worldTime.Minute;

    public void SetPaused(bool paused)
        => _timeController.SetPaused(paused);

    public void SetConsole(bool active, bool stopGame)
        => _timeController.SetConsole(active, stopGame);

    public void SetGameTimeScale(float scale)
        => _timeController.SetGameTimeScale(scale);

    public void Tick(float deltaTime)
    {
        if (IsPaused.Value) return;
        _worldTime.Tick(deltaTime);
    }

    public void Load(LocalGameTimeData data)
    {
        _worldTime.Load(data);
    }

    public LocalGameTimeData Save()
    {
        return _worldTime.Save();
    }
}


[Serializable]
public class LocalGameTimeData
{
    public double WorldSeconds = 0;
}