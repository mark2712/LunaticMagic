using System;
using System.Collections.Generic;

namespace Entities
{
    /// <summary>
    /// Локальная шина сигналов сущности (уровень L1).
    /// Обеспечивает слабую связность (Decoupling) между компонентами одной сущности:
    /// компоненты публикуют события (например, получение урона, смерть, прыжок),
    /// а другие компоненты (ИИ, лут, аниматор L3) реагируют на них, не зная об источнике.
    /// </summary>
    public class EntityEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        /// <summary>Подписаться на событие определенного типа.</summary>
        public void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _subscribers[type] = list;
            }
            list.Add(handler);
        }

        /// <summary>Отписаться от события определенного типа.</summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                {
                    _subscribers.Remove(type);
                }
            }
        }

        /// <summary>Опубликовать событие для всех подписанных компонентов данной сущности.</summary>
        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                // Итерируемся в обратном порядке для безопасной отписки прямо внутри обработчика
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (i < list.Count)
                    {
                        ((Action<T>)list[i]).Invoke(eventData);
                    }
                }
            }
        }

        /// <summary>Очистить все подписки (вызывается при уничтожении сущности).</summary>
        public void Clear()
        {
            _subscribers.Clear();
        }
    }
}
