using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;


namespace UITK
{
    /// <summary>
    /// 1) UI компонент, автоматически пересоздаётся при изменении props или Container или Prefab. Так же обновляется при изменении зависимых данных из Use.
    /// 2) управление реактивными подписками 
    /// 3) Управление подписками UnityEvent. Использовать если подписка на событие вне View или подписка происходит несколько раз (не в Init). Можно не использовать если события висят на View поскольку View будет уничтожен автоматически. Так же можно не использовать если подписка на событие происходит одни раз (например в Init).
    /// </summary>
    public abstract partial class UIComponent : IDisposable
    {
        protected UIController UIController = GlobalGame.UIController;
        public string Key { get; set; } = "0";
        public UIComponent Parent { get; private set; }
        public Dictionary<Type, Dictionary<string, UIComponent>> Children = new();

        public VisualElement Container { get; private set; }
        public VisualTreeAsset Template { get; private set; }
        public VisualElement View { get; private set; }

        protected List<UIComponent> OldNodes = new();
        protected List<UIComponent> NewNodes = new();

        private bool _isDisposed;
        public bool IsDisposed => _isDisposed;

        public virtual void Init() { }
        public virtual void Render() { }
        public virtual void Destroy() { }
        public virtual void OnActive() { }
        public virtual void OnDisable() { }

        public void ScheduleRender(UIComponent component)
        {
            UIController.ScheduleRender(component);
        }

        public void ScheduleRender()
        {
            ScheduleRender(this);
        }

        public UIComponent CreateRootNode()
        {
            ScheduleRender();
            return this;
        }

        public UIComponent AddParent(UIComponent parent)
        {
            Parent = parent;
            return this;
        }

        public UIComponent AddTemplate(VisualTreeAsset template, VisualElement container)
        {
            Template = template;
            Container = container;
            return this;
        }

        public UIComponent CreateView()
        {
            if (Template != null)
            {
                // Создаем копию из UXML
                View = Template.CloneTree();

                // По умолчанию CloneTree возвращает TemplateContainer, который растягивается на родителя.
                // Часто полезно дать ему имя или класс для стилизации.
                View.name = $"{GetType().Name}_{Key}";
                View.AddToClassList("UITK_View");
            }
            else
            {
                // Если шаблона нет, создаем пустой контейнер (как пустой GameObject)
                View = new VisualElement();
                View.name = $"{GetType().Name}_{Key}";
            }

            // Добавляем в родительский элемент (аналог transform.SetParent)
            Container.Add(View);

            SubscribeLanguage();
            Init();
            Render();
            return this;
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

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            CleanupNodes();

            foreach (var dict in Children.Values)
            {
                foreach (var child in dict.Values.ToList())
                    child.Dispose();
            }
            Children.Clear();

            Destroy();

            EventsDispose();

            if (Parent != null)
            {
                Dictionary<string, UIComponent> dict = Parent.Children[GetType()];
                dict.Remove(Key);
            }

            // Удаление View из UI Toolkit
            View?.RemoveFromHierarchy();
            View = null;

            ResourcesVisualTreeAssetDispose();
            LocalizationDispose();
        }
    }
}