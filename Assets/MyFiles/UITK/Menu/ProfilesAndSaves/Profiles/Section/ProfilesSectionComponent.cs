using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfilesSectionComponent : UIComponent<Type>
    {
        public ProfilesSectionComponent(Type props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _nowProfile;
        private VisualTreeAsset _saveButton;
        private VisualTreeAsset _form;
        private VisualTreeAsset _list;

        public override void Init()
        {
            _nowProfile = LoadUITK("Menu/ProfilesAndSaves/Profile/Now/index.uxml");
            _saveButton = LoadUITK("Menu/ProfilesAndSaves/Saves/SaveButton/index.uxml");
            _form = LoadUITK("Menu/ProfilesAndSaves/Profiles/Form/index.uxml");
            _list = LoadUITK("Menu/ProfilesAndSaves/Profiles/List/index.uxml");
        }

        public override void Render()
        {
            Node<NowProfileComponent, string>(GlobalGame.Session.Profile.ProfileId.Value, _nowProfile, View.Q("NowProfileContainer"), "now_profile");
            Node<SaveButtonComponent, Type>(null, _saveButton, View.Q("SaveButtonContainer"), "save_button");
            Node<CreateProfileFormComponent, Type>(null, _form, View.Q("ProfilesFormContainer"), "form");
            Node<ProfilesListComponent, Type>(null, _list, View.Q("ProfilesListContainer"), "list");
        }
    }
}