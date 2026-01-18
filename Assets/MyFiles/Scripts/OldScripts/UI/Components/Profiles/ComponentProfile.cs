using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace UICanvas
{
    public class ComponentProfile : UIComponent<string>
    {
        private GameProfile Profile;
        private Button Button;
        private TMPro.TMP_Text ProfileIdText;
        private TMPro.TMP_Text DifficultyText;
        private TMPro.TMP_Text ProfileTypeText;

        // private int StateComponentProfile = 0;

        public ComponentProfile(string props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            // Profile = GlobalGame.Profiles.Profiles[Props];
            // Button = View.transform.Find("Button").GetComponent<Button>();
            // ProfileIdText = View.transform.Find("ProfileId").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            // DifficultyText = View.transform.Find("Difficulty").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            // ProfileTypeText = View.transform.Find("ProfileType").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
            // UnityEventSubscribe(Button.onClick, OnClickButton);
        }

        public override void Render()
        {
            // ReactiveProperty<string> Name = Use(Profile.Name);
            // ReactiveProperty<DifficultyGame> Difficulty = Use(Profile.Difficulty);
            // ReactiveProperty<ProfileTypes> ProfileType = Use(Profile.ProfileType);

            // StateComponentProfile++;
            // Debug.Log($"ПРОФИЛЬ {Profile.Name} id {Profile.ProfileId} \n State: {StateComponentProfile}");
            // // тут обновляю текст название профиля
            // ProfileIdText.text = $"{Name.Value} \n State: {StateComponentProfile}";
            // DifficultyText.text = $"{Difficulty.Value}";
            // ProfileTypeText.text = $"{ProfileType.Value}";

            // if (!IsActive)
            // {
            //     ProfileIdText.text = $"Профиль отключен";
            //     return;
            // }
        }

        // public override void Destroy()
        // {
        //     Button.onClick.RemoveListener(ChangeActive);
        // }

        public void OnClickButton()
        {
            ChangeActive();
        }
    }
}