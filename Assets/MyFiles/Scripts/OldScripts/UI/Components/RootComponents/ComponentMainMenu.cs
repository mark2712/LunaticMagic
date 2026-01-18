using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UICanvas
{
    public class ComponentMainMenu : UIComponent<Type>
    {
        public GameObject ProfilesContainer;
        public GameObject GameProfilesPrefab;
        public ComponentMainMenu(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            ProfilesContainer = View.transform.Find("GameProfilesContainer").gameObject;
            GameProfilesPrefab = PrefabManager.LoadUI("Profiles/GameProfiles");
        }

        public override void Render()
        {
            Node<ComponentProfiles, Type>(null, GameProfilesPrefab, ProfilesContainer, "0");
        }
    }
}