using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UITK
{
    /* Управление событиями */
    public abstract partial class UIComponent : IDisposable
    {
        // Список маркеров для событий UITK
        private readonly List<EventCallbackMarker> _eventSubscriptions = new();

        protected void RegisterCallback<TEventType>(VisualElement element, EventCallback<TEventType> callback) where TEventType : EventBase<TEventType>, new()
        {
            element.RegisterCallback(callback);
            _eventSubscriptions.Add(new EventCallbackMarker<TEventType>(element, callback));
        }

        // При Dispose нужно вызвать отписку
        // В методе Dispose добавьте: 
        // foreach(var sub in _eventSubscriptions) sub.Dispose(); _eventSubscriptions.Clear();

        private interface EventCallbackMarker : IDisposable { }

        private class EventCallbackMarker<T> : EventCallbackMarker where T : EventBase<T>, new()
        {
            private readonly VisualElement _element;
            private readonly EventCallback<T> _callback;

            public EventCallbackMarker(VisualElement element, EventCallback<T> callback)
            {
                _element = element;
                _callback = callback;
            }

            public void Dispose()
            {
                _element?.UnregisterCallback(_callback);
            }
        }

        protected void EventsDispose()
        {
            foreach (var sub in _eventSubscriptions)
            {
                sub.Dispose();
            }
            _eventSubscriptions.Clear();
        }
    }
}



// public abstract partial class UIComponent : IDisposable
// {
//     protected void UnityEventSubscribe(UnityEngine.Events.UnityEvent unityEvent, UnityEngine.Events.UnityAction action)
//     {
//         unityEvent.AddListener(action);

//         _subscriptions.Add(new UnityEventSubscription(unityEvent, action));
//     }

//     private class UnityEventSubscription : ISubscriptionMarker
//     {
//         private readonly UnityEngine.Events.UnityEvent _event;
//         private readonly UnityEngine.Events.UnityAction _action;

//         public UnityEventSubscription(UnityEngine.Events.UnityEvent unityEvent, UnityEngine.Events.UnityAction action)
//         {
//             _event = unityEvent;
//             _action = action;
//         }

//         public void Dispose()
//         {
//             _event.RemoveListener(_action);
//         }

//         public bool IsSameSource(object source) => ReferenceEquals(_event, source);
//     }

//     protected void UnityEventSubscribe<T>(UnityEngine.Events.UnityEvent<T> unityEvent, UnityEngine.Events.UnityAction<T> action)
//     {
//         unityEvent.AddListener(action);
//         _subscriptions.Add(new UnityEventSubscription<T>(unityEvent, action));
//     }

//     private class UnityEventSubscription<T> : ISubscriptionMarker
//     {
//         private readonly UnityEngine.Events.UnityEvent<T> _event;
//         private readonly UnityEngine.Events.UnityAction<T> _action;

//         public UnityEventSubscription(UnityEngine.Events.UnityEvent<T> unityEvent, UnityEngine.Events.UnityAction<T> action)
//         {
//             _event = unityEvent;
//             _action = action;
//         }

//         public void Dispose()
//         {
//             _event.RemoveListener(_action);
//         }

//         public bool IsSameSource(object source) => ReferenceEquals(_event, source);
//     }
// }