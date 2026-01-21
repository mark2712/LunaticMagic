using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class LocalizationModulesDebugComponent : UIComponent<Type>
    {
        public LocalizationModulesDebugComponent(Type props, string key = "0") : base(props, key) { }
        private ResourceSystem.IResourceManager<Dictionary<string, string>> _text;
        private readonly ReactiveProperty<DateTime> _update = new(DateTime.UtcNow);

        public override void Init()
        {
            _text = Localization.Text.Manager;
            Use(_update);

            var refreshBtn = View.Q<Button>("RefreshButton");
            RegisterCallback<ClickEvent>(refreshBtn, _ =>
            {
                _update.Value = DateTime.UtcNow;
            });
        }

        public override void Render()
        {
            var list = View.Q("ModulesList");
            list.Clear();

            foreach (var pair in _text.Assets)
            {
                string path = pair.Key;
                var module = pair.Value;

                var row = new VisualElement();
                row.AddToClassList("module-row");

                row.Add(new Label(path));
                row.Add(new Label($"RefCount: {module.RefCount}"));
                row.Add(new Label($"LastUse: {module.LastUseTime:HH:mm:ss}"));

                list.Add(row);
            }
        }
    }
}