using System;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    public static class EventQueue
    {
        private static Queue<Func<State>> queue = new Queue<Func<State>>();

        public static void AddEvent(Func<State> action)
        {
            queue.Enqueue(action);
        }

        public static void ProcessEvents()
        {
            while (queue.Count > 0)
            {
                State newState = queue.Dequeue().Invoke();
                if (newState != null)
                {
                    if (newState?.SM == null)
                    {
                        Debug.LogError($"Нет SM у {newState}");
                    }
                    newState?.SM.GoToState(newState);
                }
            }
        }
    }
}