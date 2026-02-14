using UniRx;
using System;
using UnityEngine.UIElements;

namespace UITK
{
    public class LanguageSelectComponent : UIComponent<Type>
    {
        public LanguageSelectComponent(Type props, string key = "0") : base(props, key) { }

        private IReadOnlyReactiveCollection<string> _languageList;
        private IReadOnlyReactiveProperty<string> _currentLang;

        public override void Init()
        {
            _languageList = Use(Localization.LanguageList);
            _currentLang = Use(Localization.Language);
        }

        public override void Render()
        {
            var container = View.Q("LanguagesContainer");
            container.Clear();

            foreach (var lang in _languageList)
            {
                var btn = new Button { text = lang.ToUpperInvariant() };
                btn.AddToClassList("button");
                btn.AddToClassList("button_s");

                if (lang == _currentLang.Value)
                    btn.AddToClassList("button_gold");

                RegisterCallback<ClickEvent>(btn, _ =>
                    Localization.SetLanguage(lang)
                );

                container.Add(btn);
            }
        }
    }
}