using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class NowProfileComponent : UIComponent<Type>
    {
        public NowProfileComponent(Type props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _saveButtonComponent;

        private Label _nameLabel;
        private Label _difficultyLabel;
        private Label _typeLabel;

        public override void Init()
        {
            _saveButtonComponent = LoadUITK("Menu/ProfilesAndSaves/Saves/SaveButton/index.uxml");

            _nameLabel = View.Q<Label>("NameLabel");
            _difficultyLabel = View.Q<Label>("DifficultyLabel");
            _typeLabel = View.Q<Label>("TypeLabel");

            Use(GlobalGame.SessionProfuleId); // именно по этому флагу можно понять что профиль сессии сменися
        }

        public override void Render()
        {
            var _profile = GlobalGame.Session.Profile;

            if (_profile == null)
            {
                _nameLabel.text = Text("ui/Profile", "NoProfile");
                _difficultyLabel.text = "";
                _typeLabel.text = "";
                return;
            }

            Use(_profile.Name);
            Use(_profile.Difficulty);
            Use(_profile.ProfileType);

            _nameLabel.text = $"{Text("ui/Profile", "Name")}: {_profile.Name.Value}";
            _difficultyLabel.text = $"{Text("ui/Profile", "Difficulty")}: " + Text("ui/DifficultyGame", ((int)_profile.Difficulty.Value).ToString());
            _typeLabel.text = $"{Text("ui/Profile", "Type")}: " + Text("ui/ProfileTypes", ((int)_profile.ProfileType.Value).ToString());

            Node<SaveButtonComponent, Type>(null, _saveButtonComponent, View.Q("SaveButton"));
        }
    }
}
