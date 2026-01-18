using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UITK
{
    public abstract class UIComponent<TProps> : UIComponent
    {
        public TProps Props { get; protected set; }
        public UIComponent(TProps props, string key = "0")
        {
            Props = props;
            Key = key;
        }

        public UIComponent Node<TNewUIComponent, TNewProps>(TNewProps props, VisualTreeAsset template, VisualElement container, string key = "0")
        where TNewUIComponent : UIComponent<TNewProps>
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

                // Проверка props и зависимостей
                if ((props == null || component.Props.Equals(props)) && component.Template == template && component.Container == container)
                {
                    // В UITK аналог SetAsLastSibling -> BringToFront
                    component.View.BringToFront();

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
            newComponent.AddParent(this).AddTemplate(template, container);
            childKeyAndComponent[key] = newComponent;

            newComponent.CreateView();
            NewNodes.Add(newComponent);

            return newComponent;
        }
    }
}
