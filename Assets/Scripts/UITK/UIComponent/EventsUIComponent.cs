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
