using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public class UIComponentRoot : UIComponent<Type>
    {
        // public VisualTreeAsset MainMenu;
        // public VisualTreeAsset Console;
        // public VisualTreeAsset Loading;
        public VisualTreeAsset DebugMenu;

        // private ReactiveProperty<bool> MainMenuState;
        // private ReactiveProperty<bool> ConsoleState;
        // private ReactiveProperty<bool> LoadingState;
        private IReadOnlyReactiveProperty<bool> DebugMenuState;

        public UIComponentRoot(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            // Use(GlobalGame.Localization.Language);

            // MainMenu = VisualTreeAssetManager.LoadUITK("MainMenu");
            // Console = VisualTreeAssetManager.LoadUITK("Console");
            // Loading = VisualTreeAssetManager.LoadUITK("Loading");
            DebugMenu = VisualTreeAssetManager.LoadUITK("Debug/DebugMenu");

            // MainMenuState = Use(GlobalGame.UIGlobalState.MainMenu);
            // ConsoleState = Use(GlobalGame.UIGlobalState.Console);
            // LoadingState = Use(GlobalGame.UIGlobalState.Loading);
            DebugMenuState = Use(GlobalGame.UIGlobalState.DebugMenu);
        }

        public override void Render()
        {
            RootVisualElementStyles(View);

            // if (MainMenuState.Value) { Node<ComponentMainMenu, Type>(null, MainMenu, View, "0"); }
            // if (LoadingState.Value) { Node<ComponentLoading, Type>(null, Loading, View, "0"); }
            // if (ConsoleState.Value) { Node<ComponentConsole, Type>(null, Console, View, "0"); }
            if (DebugMenuState.Value) { Node<UIComponentDebugMenu, Type>(null, DebugMenu, View, "0"); }
        }

        private void RootVisualElementStyles(VisualElement visualElement)
        {
            visualElement.style.flexGrow = 1;
            visualElement.style.flexDirection = FlexDirection.Column;
            visualElement.style.position = Position.Relative;
        }
    }
}