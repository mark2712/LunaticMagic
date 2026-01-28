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

                if (lang == _currentLang.Value)
                    btn.AddToClassList("selected");

                RegisterCallback<ClickEvent>(btn, _ =>
                    Localization.SetLanguage(lang)
                );

                container.Add(btn);
            }
        }
    }

    // public class LanguageSelectComponent : UIComponent<Type>
    // {
    //     public LanguageSelectComponent(Type props, string key = "0") : base(props, key) { }
    //     private IReadOnlyReactiveCollection<string> LanguageList;
    //     private IReadOnlyReactiveProperty<string> CurrentLang;

    //     public override void Init()
    //     {
    //         LanguageList = Use(Localization.LanguageList);
    //         CurrentLang = Use(Localization.Language);
    //     }

    //     public override void Render()
    //     {
    //         var container = View.Q("LanguagesContainer");
    //         container.Clear();

    //         foreach (var lang in LanguageList)
    //         {
    //             var btn = new Button
    //             {
    //                 text = lang.ToUpperInvariant()
    //             };

    //             if (lang == CurrentLang.Value)
    //                 btn.AddToClassList("selected");

    //             RegisterCallback<ClickEvent>(btn, _ =>
    //             {
    //                 Localization.SetLanguage(lang);
    //             });

    //             container.Add(btn);
    //         }
    //     }
    // }
}