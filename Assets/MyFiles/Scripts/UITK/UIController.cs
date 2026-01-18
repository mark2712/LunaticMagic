using System.Collections.Generic;


namespace UITK
{
    /// <summary>
    /// Отвечает за жизненный цикл компонентов (не путать с UIManager который отвечает за Root компоненты)
    /// <list type="bullet">
    /// <item>
    /// ScheduleRender добавялет компонент в очередь на рендер 
    /// </item>
    /// <item>
    /// RenderAll вызывает рендер изменившихся или новых компонентов (сейчас в LateUpdate)
    /// </item>
    /// </list>
    /// </summary>
    public class UIController
    {
        private readonly List<UIComponent> _componentsToRender = new();
        private readonly HashSet<UIComponent> _scheduledSet = new();

        // добавить компонент в очередь на рендер
        public void ScheduleRender(UIComponent component)
        {
            if (component == null) return;

            // быстрый тест на дубль
            if (_scheduledSet.Contains(component)) return;

            // если уже есть запланированный предок компонента — не добавляем
            foreach (var scheduled in _componentsToRender)
            {
                if (IsAncestor(scheduled, component))
                    return;
            }

            // удаляем из очереди всех предзапланированных потомков component
            for (int i = _componentsToRender.Count - 1; i >= 0; i--)
            {
                var c = _componentsToRender[i];
                if (IsAncestor(component, c))
                {
                    _scheduledSet.Remove(c);
                    _componentsToRender.RemoveAt(i);
                }
            }

            _componentsToRender.Add(component);
            _scheduledSet.Add(component);
        }

        // вызывает рендер изменившихся или новых компонентов
        public void RenderAll()
        {
            foreach (var component in _componentsToRender)
            {
                if (component.View == null)
                {
                    component.CreateView();
                    component.CleanupNodes();
                }
                else
                {
                    component.Render();
                    component.CleanupNodes();
                }
            }
            _componentsToRender.Clear();
            _scheduledSet.Clear();
        }

        private bool IsAncestor(UIComponent possibleAncestor, UIComponent descendant)
        {
            if (possibleAncestor == null || descendant == null) return false;

            var current = descendant.Parent;
            while (current != null)
            {
                if (current == possibleAncestor) return true;
                current = current.Parent;
            }
            return false;
        }
    }
}
