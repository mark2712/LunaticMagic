using System;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class LocalizationModulesDebugComponent : UIComponent<Type>
    {
        public LocalizationModulesDebugComponent(Type props, string key = "0") : base(props, key) { }
        private ITextLocalization _text;
        private readonly ReactiveProperty<DateTime> _update = new(DateTime.UtcNow);

        public override void Init()
        {
            _text = Localization.Text;
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

            foreach (var pair in _text.Modules)
            {
                string path = pair.Key;
                LocalizationModule module = pair.Value;

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