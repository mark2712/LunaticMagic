using System;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

namespace UITK
{
    public sealed class CreateProfileFormComponent : UIComponent<Type>
    {
        public CreateProfileFormComponent(Type props, string key = "0") : base(props, key) { }

        private TextField _nameField;
        private RadioButtonGroup _difficultyGroup;
        private RadioButtonGroup _profileTypeGroup;

        private readonly List<RadioButton> _difficultyRadios = new();
        private readonly List<RadioButton> _profileTypeRadios = new();

        public override void Init()
        {
            _nameField = View.Q<TextField>("NameField");

            BuildDifficultyRadios();
            BuildProfileTypeRadios();

            var createButton = View.Q<Button>("CreateButton");
            RegisterCallback<ClickEvent>(createButton, _ => OnCreate());
        }

        public override void Render()
        {
            for (int i = 0; i < _difficultyRadios.Count; i++)
            {
                _difficultyRadios[i].text = Text($"ui/DifficultyGame.{i}");
            }

            for (int i = 0; i < _profileTypeRadios.Count; i++)
            {
                _profileTypeRadios[i].text = Text($"ui/ProfileTypes.{i}");
            }
        }

        private void BuildDifficultyRadios()
        {
            var container = View.Q("DifficultyContainer");

            _difficultyGroup = new RadioButtonGroup();
            container.Add(_difficultyGroup);

            foreach (DifficultyGame diff in Enum.GetValues(typeof(DifficultyGame)))
            {
                var radio = new RadioButton();
                _difficultyGroup.Add(radio);
                _difficultyRadios.Add(radio);
            }
        }

        private void BuildProfileTypeRadios()
        {
            var container = View.Q("ProfileTypeContainer");

            _profileTypeGroup = new RadioButtonGroup();
            container.Add(_profileTypeGroup);

            foreach (ProfileTypes value in Enum.GetValues(typeof(ProfileTypes)))
            {
                var radio = new RadioButton();
                _profileTypeGroup.Add(radio);
                _profileTypeRadios.Add(radio);
            }

            _profileTypeGroup.value = (int)ProfileTypes.User;
        }

        // Создать новый профиль
        private void OnCreate()
        {
            string name = _nameField.value;
            var difficulty = (DifficultyGame)Enum.GetValues(typeof(DifficultyGame)).GetValue(_difficultyGroup.value);
            var profileType = (ProfileTypes)Enum.GetValues(typeof(ProfileTypes)).GetValue(_profileTypeGroup.value);
            GlobalGame.Profiles.Create(name, profileType).Difficulty.Value = difficulty;
        }
    }
}
