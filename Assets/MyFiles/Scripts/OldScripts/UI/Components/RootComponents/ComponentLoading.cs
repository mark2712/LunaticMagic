using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UICanvas
{
    public class ComponentLoading : UIComponent<Type>
    {
        // public GameObject ProfilesContainer;
        // public GameObject GameProfilesPrefab;
        public ComponentLoading(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            // ProfilesContainer = View.transform.Find("GameProfilesContainer").gameObject;
            // GameProfilesPrefab = PrefabManager.Load("Prefabs/UI/GameProfiles");
        }

        public override void Render()
        {
            // Node<ComponentProfiles, Type>(null, GameProfilesPrefab, ProfilesContainer, "0");
        }
    }
}