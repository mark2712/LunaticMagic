using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UICanvas
{
    public class ComponentDebugMenu : UIComponent<Type>
    {
        public GameObject AllContainer;
        public GameObject GameProfilesPrefab;

        public ComponentDebugMenu(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            AllContainer = View.transform.Find("ScrollView").Find("Viewport").Find("Content").gameObject;
            GameProfilesPrefab = PrefabManager.LoadUI("Profiles/GameProfiles");
        }

        public override void Render()
        {
            Node<ComponentProfiles, Type>(null, GameProfilesPrefab, AllContainer, "0");
        }

        // public override void Destroy()
        // {
        //     Button.onClick.RemoveListener(ChangeActive);
        // }

        // public void OnClickButton()
        // {
        //     ChangeActive();
        // }
    }
}