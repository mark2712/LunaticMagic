using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfilesSectionComponent : UIComponent<IGameProfiles>
    {
        public ProfilesSectionComponent(IGameProfiles props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _nowProfile;
        private VisualTreeAsset _saveButton;
        private VisualTreeAsset _form;
        private VisualTreeAsset _list;

        public override void Init()
        {
            _nowProfile = LoadUITK("GlobalGame/ProfilesAndSaves/Profile/Now/index.uxml");
            _saveButton = LoadUITK("GlobalGame/ProfilesAndSaves/Saves/SaveButton/index.uxml");
            _form = LoadUITK("GlobalGame/ProfilesAndSaves/Profiles/Form/index.uxml");
            _list = LoadUITK("GlobalGame/ProfilesAndSaves/Profiles/List/index.uxml");
        }

        public override void Render()
        {
            Node<NowProfileComponent, Type>(null, _nowProfile, View.Q("NowProfileContainer"), "now_profile");
            Node<SaveButtonComponent, Type>(null, _saveButton, View.Q("SaveButtonContainer"), "save_button");
            Node<CreateProfileFormComponent, IGameProfiles>(Props, _form, View.Q("ProfilesFormContainer"), "form");
            Node<ProfilesListComponent, IGameProfiles>(Props, _list, View.Q("ProfilesListContainer"), "list");
        }
    }
}
