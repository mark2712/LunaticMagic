using System;
using UniRx;
using UnityEngine;

// Состояние процесса загрузки сессии (прогресс от 0 до 100)
public class GameLoadingProgress : IDisposable
{
    // Прогресс загрузки в процентах (0..100)
    public ReactiveProperty<float> Progress { get; } = new(0f);

    public void SetProgress(float value)
    {
        Progress.Value = Mathf.Clamp(value, 0f, 100f);
    }

    public void Reset()
    {
        Progress.Value = 0f;
    }

    public void Dispose()
    {
        Progress.Dispose();
    }
}
