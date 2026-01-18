using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateInterval
{
    private float globalTime = 0f; // общее "время игры" по FixedUpdate

    private readonly List<TickEvent> events = new();

    // [Serializable]
    public class TickEvent
    {
        public float Period;       // через сколько секунд вызывать
        public float NextInvoke;   // следующее время вызова
        public Action Callback;    // делегат
        public float LastInvoke;   // время последнего вызова
    }

    public void Subscribe(float period, Action callback)
    {
        var evt = new TickEvent
        {
            Period = period,
            Callback = callback,
            NextInvoke = globalTime + period,
            LastInvoke = globalTime
        };
        events.Add(evt);
    }

    public void Unsubscribe(Action callback)
    {
        events.RemoveAll(e => e.Callback == callback);
    }


    public void FixedUpdate()
    {
        globalTime += Time.fixedDeltaTime;

        for (int i = 0; i < events.Count; i++)
        {
            var evt = events[i];

            if (globalTime >= evt.NextInvoke)
            {
                // вызываем
                evt.Callback?.Invoke();

                evt.LastInvoke = globalTime;
                evt.NextInvoke += evt.Period;

                // если мы сильно отстали (например лаг) — догоняем (событие выполнится 1 раз даже если за это время должно было выполниться несколько)
                if (globalTime > evt.NextInvoke)
                    evt.NextInvoke = globalTime + evt.Period;
            }
        }
    }
}

// GlobalGame.UpdateInterval.Subscribe(0.5f, () => Debug.Log("TestSubscribe 0.5"));
// GlobalGame.UpdateInterval.Subscribe(1.5f, () => Debug.Log("TestSubscribe 1.5"));
// GlobalGame.UpdateInterval.Subscribe(2f, () => Debug.Log("TestSubscribe2"));