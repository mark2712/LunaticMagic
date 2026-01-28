using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public class UIComponentDebugMenu : UIComponent<Type>
    {
        public UIComponentDebugMenu(Type props, string key = "0") : base(props, key) { }
        public VisualTreeAsset VrmDebug;
        public VisualTreeAsset LocalizationModulesDebug;
        public VisualTreeAsset LanguageSelect;
        public VisualTreeAsset LanguageDropdown;
        public VisualTreeAsset ProfilesSection;

        public override void Init()
        {
            // VrmDebug = LoadUITK("Debug/VRM/VrmDebug");

            // LocalizationModulesDebug = LoadUITK("Debug/Localization/LocalizationAssetsDebug/LocalizationAssetsDebug.uxml");
            LanguageSelect = LoadUITK("Components/Language/Select/index.uxml");
            LanguageDropdown = LoadUITK("Components/Language/Dropdown/index.uxml");

            ProfilesSection = LoadUITK("Menu/ProfilesAndSaves/Profiles/Section/index.uxml");

        }

        public override void Render()
        {
            var DebugScroll = View.Q<ScrollView>("DebugScroll");

            // UIComponent UIComponenVrmDebug = Node<VrmDebugComponent, Type>(null, VrmDebug, DebugScroll, "0");

            // UIComponent UIComponentLocalizationModulesDebug = Node<AssetsDebugComponent, Type>(null, LocalizationModulesDebug, DebugScroll, "0");
            Node<LanguageSelectComponent, Type>(null, LanguageSelect, DebugScroll, "0");
            Node<LanguageDropdownComponent, Type>(null, LanguageDropdown, DebugScroll, "0");

            UIComponent UIComponentProfilesSection = Node<ProfilesSectionComponent, Type>(null, ProfilesSection, DebugScroll, "0");
        }
    }
}