using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace UICanvas
{
    /// <summary>
    /// 1) UI компонент, автоматически пересоздаётся при изменении props или Container или Prefab. Так же обновляется при изменении зависимых данных из Use.
    /// 2) управление реактивными подписками 
    /// 3) Управление подписками UnityEvent. Использовать если подписка на событие вне View или подписка происходит несколько раз (не в Init). Можно не использовать если события висят на View поскольку View будет уничтожен автоматически. Так же можно не использовать если подписка на событие происходит одни раз (например в Init).
    /// </summary>
    public abstract partial class UIComponent : IDisposable
    {
        public string Key { get; set; } = "0";
        public UIComponent Parent { get; private set; }
        public Dictionary<Type, Dictionary<string, UIComponent>> Children = new();

        protected List<UIComponent> OldNodes = new();
        protected List<UIComponent> NewNodes = new();

        public GameObject Container { get; private set; }
        public GameObject Prefab { get; private set; }
        public GameObject View { get; private set; }

        public bool IsActive { get; private set; } = true;

        public virtual void Init() { }
        public virtual void Render() { }
        public virtual void Destroy() { }
        public virtual void OnActive() { }
        public virtual void OnDisable() { }

        public void ScheduleRender(UIComponent component)
        {
            // GlobalGame.UIController.ScheduleRender(component);
        }

        public void ScheduleRender()
        {
            ScheduleRender(this);
        }

        public UIComponent AddParent(UIComponent parent)
        {
            Parent = parent;
            return this;
        }

        public UIComponent AddPrefab(GameObject prefab, GameObject container)
        {
            Prefab = prefab;
            Container = container;
            return this;
        }

        public UIComponent CreateRootNode()
        {
            ScheduleRender();
            return this;
        }

        public void Active()
        {
            if (IsActive) return;
            IsActive = true;
            OnActive();
            ScheduleRender();
        }

        public void Disable()
        {
            if (!IsActive) return;
            IsActive = false;
            OnDisable();
            ScheduleRender();
        }

        public void ChangeActive()
        {
            if (IsActive) { Disable(); } else { Active(); }
        }

        public UIComponent CreateView()
        {
            View = GameObject.Instantiate(Prefab, Container.transform);
            Init();
            Render();
            return this;
        }

        public void Dispose()
        {
            foreach (var sub in _subscriptions.ToList())
            {
                sub.Dispose();
            }
            _subscriptions.Clear();

            CleanupNodes(); // сначала очищаем устаревшие узлы

            foreach (var dict in Children.Values)
            {
                foreach (var child in dict.Values.ToList()) // ToList чтобы не было модификации коллекции при Dispose
                {
                    child.Dispose();
                }
            }
            Children.Clear();

            Destroy();

            // убираем из словаря родителя
            if (Parent != null)
            {
                var dict = Parent.Children[GetType()];
                dict.Remove(Key);
            }

            if (View != null) { GameObject.Destroy(View); }
            View = null;
        }

        /// <summary>
        /// Полное удаление дочернего компонента из дерева и из Children
        /// </summary>
        protected void RemoveNode(UIComponent component)
        {
            component.Dispose();
        }

        /// <summary>
        /// Очищает все старые узлы, которые не попали в NewNodes
        /// </summary>

        public void CleanupNodes()
        {
            foreach (var node in OldNodes.Except(NewNodes))
            {
                node.Dispose();
            }
            OldNodes = new List<UIComponent>(NewNodes);
            NewNodes.Clear();
        }
    }

    public abstract partial class UIComponent : IDisposable
    {
        private readonly List<ISubscriptionMarker> _subscriptions = new();

        // ReactiveProperty<T>
        protected ReactiveProperty<T> Use<T>(ReactiveProperty<T> property)
        {
            if (!_subscriptions.Any(s => s.IsSameSource(property)))
            {
                var sub = property.Subscribe(_ => ScheduleRender());
                _subscriptions.Add(new SubscriptionMarker<ReactiveProperty<T>>(property, sub));
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

    public abstract partial class UIComponent : IDisposable
    {
        protected void UnityEventSubscribe(UnityEngine.Events.UnityEvent unityEvent, UnityEngine.Events.UnityAction action)
        {
            unityEvent.AddListener(action);

            _subscriptions.Add(new UnityEventSubscription(unityEvent, action));
        }

        private class UnityEventSubscription : ISubscriptionMarker
        {
            private readonly UnityEngine.Events.UnityEvent _event;
            private readonly UnityEngine.Events.UnityAction _action;

            public UnityEventSubscription(UnityEngine.Events.UnityEvent unityEvent, UnityEngine.Events.UnityAction action)
            {
                _event = unityEvent;
                _action = action;
            }

            public void Dispose()
            {
                _event.RemoveListener(_action);
            }

            public bool IsSameSource(object source) => ReferenceEquals(_event, source);
        }

        protected void UnityEventSubscribe<T>(UnityEngine.Events.UnityEvent<T> unityEvent, UnityEngine.Events.UnityAction<T> action)
        {
            unityEvent.AddListener(action);
            _subscriptions.Add(new UnityEventSubscription<T>(unityEvent, action));
        }

        private class UnityEventSubscription<T> : ISubscriptionMarker
        {
            private readonly UnityEngine.Events.UnityEvent<T> _event;
            private readonly UnityEngine.Events.UnityAction<T> _action;

            public UnityEventSubscription(UnityEngine.Events.UnityEvent<T> unityEvent, UnityEngine.Events.UnityAction<T> action)
            {
                _event = unityEvent;
                _action = action;
            }

            public void Dispose()
            {
                _event.RemoveListener(_action);
            }

            public bool IsSameSource(object source) => ReferenceEquals(_event, source);
        }
    }


    public abstract class UIComponent<TProps> : UIComponent
    {
        public TProps Props { get; protected set; }
        public UIComponent(TProps props, string key = "0")
        {
            Props = props;
            Key = key;
        }

        public UIComponent Node<TNewUIComponent, TNewProps>(TNewProps props, GameObject prefab, GameObject container, string key = "0")
        where TNewUIComponent : UIComponent<TNewProps>
        // where TNewProps : IEquatable<TNewProps>
        {
            var type = typeof(TNewUIComponent);

            if (!Children.TryGetValue(type, out var childKeyAndComponent))
            {
                childKeyAndComponent = new Dictionary<string, UIComponent>();
                Children[type] = childKeyAndComponent;
            }

            if (childKeyAndComponent.TryGetValue(key, out var existing))
            {
                var component = (TNewUIComponent)existing;

                if ((props == null || component.Props.Equals(props)) && component.Prefab == prefab && component.Container == container)
                {
                    // переносим в конец
                    component.View.transform.SetAsLastSibling();
                    component.Render();
                    NewNodes.Add(component);
                    return component;
                }
                else
                {
                    RemoveNode(component);
                }
            }

            var newComponent = (TNewUIComponent)Activator.CreateInstance(typeof(TNewUIComponent), props, key)!;
            newComponent.AddParent(this).AddPrefab(prefab, container);
            childKeyAndComponent[key] = newComponent;

            newComponent.CreateView();
            NewNodes.Add(newComponent);

            return newComponent;
        }
    }
}

// public bool IsUpdate => IsActive && (Parent?.IsUpdate ?? true);
