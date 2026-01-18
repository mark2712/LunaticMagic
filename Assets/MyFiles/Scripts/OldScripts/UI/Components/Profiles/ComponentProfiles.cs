using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UICanvas
{
    public class ComponentProfiles : UIComponent<Type>
    {
        public GameObject ProfileContainer;
        public GameObject GameProfilePrefab;
        private Button Button;
        private Dictionary<string, GameProfile> Profiles = new();
        private TMPro.TMP_Text H2Text;

        // private int StateComponentProfiles = 0;

        public ComponentProfiles(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            ProfileContainer = View.transform.Find("Profiles/Viewport/Content").gameObject;
            GameProfilePrefab = PrefabManager.LoadUI("Profiles/Profile");
            GameObject H2 = View.transform.Find("H2").gameObject;
            H2Text = H2.GetComponent<TMPro.TextMeshProUGUI>();
            Button = View.transform.Find("Panel/Add").GetComponent<Button>();
            UnityEventSubscribe(Button.onClick, OnClickButton);
        }

        public override void Render()
        {
            // if (!IsActive)
            // {
            //     H2Text.text = $"Профили отключены";
            //     return;
            // }

            // Profiles = Use(GlobalGame.Profiles.Profiles).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // StateComponentProfiles++;
            // Debug.Log($"Профили {Profiles.Count} \n State: {StateComponentProfiles}");
            // H2Text.text = $"Профили {Profiles.Count} \n State: {StateComponentProfiles}";

            // foreach (var profile in Profiles)
            // {
            //     string id = profile.Value.ProfileId.Value;
            //     Node<ComponentProfile, string>(id, GameProfilePrefab, ProfileContainer, id);
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