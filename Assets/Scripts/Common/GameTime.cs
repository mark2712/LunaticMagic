using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTime
{
    public float GlobalTime { get; private set; } = 0f;
    public int GlobalSeconds { get; private set; } = 0;

    public float LocalTime { get; private set; } = 0f;
    public float LocalSeconds { get; private set; } = 0f;

    public float GameTimeScale { get; private set; } = 0f;

    public void Update(float deltaTime)
    {
        GlobalTime += deltaTime;

        // Обновляем целые секунды
        int currentSecond = Mathf.FloorToInt(GlobalTime);
        if (currentSecond != GlobalSeconds)
        {
            GlobalSeconds = currentSecond;
        }
    }
}

