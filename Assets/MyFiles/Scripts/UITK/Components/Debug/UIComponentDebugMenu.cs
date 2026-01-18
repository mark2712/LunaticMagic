using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public class UIComponentDebugMenu : UIComponent<Type>
    {
        public UIComponentDebugMenu(Type props, string key = "0") : base(props, key) { }
        public VisualTreeAsset LocalizationModulesDebug;
        public VisualTreeAsset LanguageSelect;
        public VisualTreeAsset ProfilesSection;

        public override void Init()
        {
            LocalizationModulesDebug = VisualTreeAssetManager.LoadUITK("Language/LocalizationModulesDebug");
            LanguageSelect = VisualTreeAssetManager.LoadUITK("Language/LanguageSelect");

            ProfilesSection = VisualTreeAssetManager.LoadUITK("ProfilesAndSaves/ProfilesSection");

        }

        public override void Render()
        {
            var DebugScroll = View.Q<ScrollView>("DebugScroll");
            UIComponent UIComponentLocalizationModulesDebug = Node<LocalizationModulesDebugComponent, Type>(null, LocalizationModulesDebug, DebugScroll, "0");
            UIComponent UIComponentLanguageSelect = Node<LanguageSelectComponent, Type>(null, LanguageSelect, DebugScroll, "0");
            UIComponent UIComponentProfilesSection = Node<ProfilesSectionComponent, Type>(null, ProfilesSection, DebugScroll, "0");
        }
    }
}