using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class NowProfileComponent : UIComponent<string>
    {
        public NowProfileComponent(string props, string key = "0") : base(props, key) { }

        private GameProfile _profile;

        private VisualTreeAsset _saveButtonComponent;

        private Label _nameLabel;
        private Label _difficultyLabel;
        private Label _typeLabel;

        public override void Init()
        {
            _nameLabel = View.Q<Label>("NameLabel");
            _difficultyLabel = View.Q<Label>("DifficultyLabel");
            _typeLabel = View.Q<Label>("TypeLabel");

            if (string.IsNullOrEmpty(Props))
                return;

            var profiles = Use(GlobalGame.Profiles.Profiles);

            if (!profiles.TryGetValue(Props, out _profile))
                return;

            // подписки на поля профиля
            Use(_profile.Name);
            Use(_profile.Difficulty);
            Use(_profile.ProfileType);

            _saveButtonComponent = LoadUITK("Menu/ProfilesAndSaves/Saves/SaveButton/index.uxml");
        }

        public override void Render()
        {
            if (_profile == null)
            {
                _nameLabel.text = Text("ui/Profile", "NoProfile");
                _difficultyLabel.text = "";
                _typeLabel.text = "";
                return;
            }

            _nameLabel.text = $"{Text("ui/Profile", "Name")}: {_profile.Name.Value}";
            _difficultyLabel.text = $"{Text("ui/Profile", "Difficulty")}: " + Text("ui/DifficultyGame", ((int)_profile.Difficulty.Value).ToString());
            _typeLabel.text = $"{Text("ui/Profile", "Type")}: " + Text("ui/ProfileTypes", ((int)_profile.ProfileType.Value).ToString());

            Node<SaveButtonComponent, Type>(null, _saveButtonComponent, View.Q("SaveButton"));
        }
    }
}
