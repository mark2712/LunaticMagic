using System;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public class LanguageDropdownComponent : UIComponent<Type>
    {
        public LanguageDropdownComponent(Type props, string key = "0") : base(props, key) { }

        private IReadOnlyReactiveCollection<string> _languageList;
        private IReadOnlyReactiveProperty<string> _currentLang;

        private bool _opened;

        public override void Init()
        {
            _languageList = Use(Localization.LanguageList);
            _currentLang = Use(Localization.Language);
        }

        public override void Render()
        {
            var currentBtn = View.Q<Button>("CurrentButton");
            var dropdown = View.Q("Dropdown");

            currentBtn.text = _currentLang.Value.ToUpperInvariant();

            RegisterCallback<ClickEvent>(currentBtn, _ =>
            {
                _opened = !_opened;
                dropdown.EnableInClassList("hidden", !_opened);
            });

            dropdown.Clear();

            foreach (var lang in _languageList)
            {
                var btn = new Button { text = lang.ToUpperInvariant() };

                if (lang == _currentLang.Value)
                    btn.AddToClassList("selected");

                RegisterCallback<ClickEvent>(btn, _ =>
                {
                    Localization.SetLanguage(lang);
                    _opened = false;
                    dropdown.AddToClassList("hidden");
                });

                dropdown.Add(btn);
            }
        }
    }
}