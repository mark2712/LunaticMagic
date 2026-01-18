using System;
using UniRx;
using UnityEngine;

namespace UICanvas
{
    public class ComponentRoot : UIComponent<Type>
    {
        public GameObject MainMenu;
        public GameObject Console;
        public GameObject Loading;
        public GameObject DebugMenu;

        private ReactiveProperty<bool> MainMenuState;
        private ReactiveProperty<bool> ConsoleState;
        private ReactiveProperty<bool> LoadingState;
        private ReactiveProperty<bool> DebugMenuState;

        public ComponentRoot(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            MainMenu = PrefabManager.LoadUI("MainMenu");
            Console = PrefabManager.LoadUI("Console");
            Loading = PrefabManager.LoadUI("Loading");
            DebugMenu = PrefabManager.LoadUI("DebugMenu");

            MainMenuState = Use(GlobalGame.UIGlobalState.MainMenu);
            ConsoleState = Use(GlobalGame.UIGlobalState.Console);
            LoadingState = Use(GlobalGame.UIGlobalState.Loading);
            DebugMenuState = Use(GlobalGame.UIGlobalState.DebugMenu);
        }

        public override void Render()
        {
            if (MainMenuState.Value) { Node<ComponentMainMenu, Type>(null, MainMenu, View, "0"); }
            if (LoadingState.Value) { Node<ComponentLoading, Type>(null, Loading, View, "0"); }
            if (ConsoleState.Value) { Node<ComponentConsole, Type>(null, Console, View, "0"); }
            if (DebugMenuState.Value) { Node<ComponentDebugMenu, Type>(null, DebugMenu, View, "0"); }
        }
    }
}