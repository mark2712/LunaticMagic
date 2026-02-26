using System;
using System.Linq;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public class LanguageDropdownComponent : UIComponent<Type>
    {
        public LanguageDropdownComponent(Type props, string key = "0") : base(props, key) { }

        private IReadOnlyReactiveCollection<string> _languageList;
        private IReadOnlyReactiveProperty<string> _currentLang;

        private DropdownField _dropdown;

        private bool _opened;

        public override void Init()
        {
            _languageList = Use(Localization.LanguageList);
            _currentLang = Use(Localization.Language);

            _dropdown = View.Q<DropdownField>("LanguageDropdown");

            RegisterCallback<ChangeEvent<string>>(_dropdown, evt =>
            {
                if (evt.newValue != _currentLang.Value)
                {
                    Localization.SetLanguage(evt.newValue);
                }
            });
        }

        public override void Render()
        {
            // choices
            _dropdown.choices = _languageList.ToList();

            // selected value
            _dropdown.SetValueWithoutNotify(_currentLang.Value);
        }
    }
}