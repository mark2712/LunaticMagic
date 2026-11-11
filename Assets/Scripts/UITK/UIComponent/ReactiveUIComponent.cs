using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;


namespace UITK
{
    /* Управление реактивностью  */
    public abstract partial class UIComponent : IDisposable
    {
        private readonly List<ISubscriptionMarker> _subscriptions = new();

        // ReactiveProperty<T>
        protected IReadOnlyReactiveProperty<T> Use<T>(IReadOnlyReactiveProperty<T> property)
        {
            if (!_subscriptions.Any(s => s.IsSameSource(property)))
            {
                var sub = property.Subscribe(_ => ScheduleRender());
                _subscriptions.Add(new SubscriptionMarker<IReadOnlyReactiveProperty<T>>(property, sub));
            }

            return property;
        }

        // ReactiveCollection<T>
        protected IReadOnlyReactiveCollection<T> Use<T>(IReadOnlyReactiveCollection<T> collection)
        {
            if (!_subscriptions.Any(s => s.IsSameSource(collection)))
            {
                var sub = collection.ObserveCountChanged()
                    .Subscribe(_ => ScheduleRender());
                _subscriptions.Add(new SubscriptionMarker<IReadOnlyReactiveCollection<T>>(collection, sub));
            }

            return collection;
        }

        // ReactiveDictionary<TKey, TValue>
        protected IReadOnlyReactiveDictionary<TKey, TValue> Use<TKey, TValue>(IReadOnlyReactiveDictionary<TKey, TValue> dict)
        {
            if (!_subscriptions.Any(s => s.IsSameSource(dict)))
            {
                var sub = new CompositeDisposable
            {
                dict.ObserveAdd().Subscribe(_ => ScheduleRender()),
                dict.ObserveRemove().Subscribe(_ => ScheduleRender()),
                dict.ObserveReplace().Subscribe(_ => ScheduleRender()),
                dict.ObserveReset().Subscribe(_ => ScheduleRender()),
            };
                _subscriptions.Add(new SubscriptionMarker<IReadOnlyReactiveDictionary<TKey, TValue>>(dict, sub));
            }

            return dict;
        }

        // универсальный маркер
        private interface ISubscriptionMarker : IDisposable
        {
            bool IsSameSource(object source);
        }

        private class SubscriptionMarker<TSource> : ISubscriptionMarker
        {
            public readonly TSource Source;
            private readonly IDisposable _disposable;

            public SubscriptionMarker(TSource source, IDisposable disposable)
            {
                Source = source;
                _disposable = disposable;
            }

            public void Dispose() => _disposable.Dispose();

            public bool IsSameSource(object source) => ReferenceEquals(Source, source);
        }
    }
}