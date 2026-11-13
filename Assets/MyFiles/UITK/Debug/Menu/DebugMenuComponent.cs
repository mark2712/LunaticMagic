using System;
using ResourceSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public class DebugMenuComponent : UIComponent<Type>
    {
        public DebugMenuComponent(Type props, string key = "0") : base(props, key) { }

        private IResourceBinding<IGameProfiles> _profilesBinding;

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

            _profilesBinding = ResourceManager.Profiles.Bind("profiles");
        }

        public override void Render()
        {
            var DebugScroll = View.Q<ScrollView>("DebugScroll");

            Node<LanguageSelectComponent, Type>(null, LanguageSelect, DebugScroll, "0");
            Node<LanguageDropdownComponent, Type>(null, LanguageDropdown, DebugScroll, "0");

            if (_profilesBinding?.Resource != null)
            {
                Node<ProfilesSectionComponent, IGameProfiles>(_profilesBinding.Resource, ProfilesSection, DebugScroll, "profiles_section");
            }
        }

        public override void Destroy()
        {
            _profilesBinding?.Dispose();
            _profilesBinding = null;
        }
    }
}
