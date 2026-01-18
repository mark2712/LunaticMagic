using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfilesSectionComponent : UIComponent<Type>
    {
        public ProfilesSectionComponent(Type props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _saveButton;
        private VisualTreeAsset _form;
        private VisualTreeAsset _list;

        public override void Init()
        {
            _saveButton = VisualTreeAssetManager.LoadUITK("ProfilesAndSaves/SaveButton");
            _form = VisualTreeAssetManager.LoadUITK("ProfilesAndSaves/ProfilesForm");
            _list = VisualTreeAssetManager.LoadUITK("ProfilesAndSaves/ProfilesList");
        }

        public override void Render()
        {
            Node<SaveButtonComponent, Type>(null, _saveButton, View.Q("SaveButtonContainer"), "save_button");
            Node<CreateProfileFormComponent, Type>(null, _form, View.Q("ProfilesFormContainer"), "form");
            Node<ProfilesListComponent, Type>(null, _list, View.Q("ProfilesListContainer"), "list");
        }
    }
}