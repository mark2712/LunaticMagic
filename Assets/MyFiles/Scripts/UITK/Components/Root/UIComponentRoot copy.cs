// using System;
// using UniRx;
// using UnityEngine;
// using UnityEngine.UIElements;

// namespace UITK
// {
//     public class UIComponentRoot : UIComponent<Type>
//     {
//         public VisualTreeAsset MainMenu;
//         public VisualTreeAsset Console;
//         public VisualTreeAsset Loading;
//         public VisualTreeAsset DebugMenu;

//         private ReactiveProperty<bool> MainMenuState;
//         private ReactiveProperty<bool> ConsoleState;
//         private ReactiveProperty<bool> LoadingState;
//         private ReactiveProperty<bool> DebugMenuState;

//         public VisualTreeAsset ComponentDebugElem;

//         public UIComponentRoot(Type props, string key = "0") : base(props, key) { }

//         public override void Init()
//         {
//             // MainMenu = VisualTreeAssetManager.LoadUITK("MainMenu");
//             // Console = VisualTreeAssetManager.LoadUITK("Console");
//             // Loading = VisualTreeAssetManager.LoadUITK("Loading");
//             DebugMenu = VisualTreeAssetManager.LoadUITK("Debug/DebugMenu");

//             MainMenuState = Use(GlobalGame.UIGlobalState.MainMenu);
//             ConsoleState = Use(GlobalGame.UIGlobalState.Console);
//             LoadingState = Use(GlobalGame.UIGlobalState.Loading);
//             DebugMenuState = Use(GlobalGame.UIGlobalState.DebugMenu);

//             ComponentDebugElem = VisualTreeAssetManager.LoadUITK("Debug/DebugElem");
//         }

//         public override void Render()
//         {
//             RootVisualElementStyles(View);

//             // if (MainMenuState.Value) { Node<ComponentMainMenu, Type>(null, MainMenu, View, "0"); }
//             // if (LoadingState.Value) { Node<ComponentLoading, Type>(null, Loading, View, "0"); }
//             // if (ConsoleState.Value) { Node<ComponentConsole, Type>(null, Console, View, "0"); }
//             // if (DebugMenuState.Value) { Node<UIComponentDebugMenu, Type>(null, DebugMenu, View, "0"); }

//             if (DebugMenuState.Value)
//             {
//                 UIComponent UIComponentDebugElem = Node<UIComponentDebugElem, Type>(null, ComponentDebugElem, View, "0");
//                 RootVisualElementStyles(UIComponentDebugElem.View);
//             }
//         }

//         private void RootVisualElementStyles(VisualElement visualElement)
//         {
//             visualElement.style.flexGrow = 1;
//             visualElement.style.flexDirection = FlexDirection.Column;
//             visualElement.style.position = Position.Relative;
//         }
//     }
// }











// using System;
// using UnityEngine;
// using UnityEngine.UIElements;

// namespace UITK
// {
//     public class UIComponentDebugMenu : UIComponent<Type>
//     {
//         public VisualTreeAsset DebugElem;

//         public UIComponentDebugMenu(Type props, string key = "0") : base(props, key) { }

//         public override void Init()
//         {
//             DebugElem = VisualTreeAssetManager.LoadUITK("Debug/DebugElem");
//         }

//         public override void Render()
//         {
//             // View.style.flexGrow = 1;
//             // View.style.position = Position.Relative;
//             // View.style.width = Length.Percent(100);
//             // View.style.height = Length.Percent(100);
//             // View.style.flexGrow = 1;
//             View.style.flexGrow = 1;
//             View.style.flexDirection = FlexDirection.Column;
//             View.style.position = Position.Relative;

//             VisualElement root2 = View.Q<VisualElement>("", "root2");
//             Debug.Log(root2);

//             UIComponent UIComponentProfiles = Node<UIComponentDebugElem, Type>(null, DebugElem, root2, "0");
//             // UIComponentProfiles.View.StretchToParentSize();
//             // UIComponentProfiles.View.style.width = Length.Percent(100);
//             // UIComponentProfiles.View.style.height = Length.Percent(100);
//             UIComponentProfiles.View.style.flexGrow = 1;
//             UIComponentProfiles.View.style.flexDirection = FlexDirection.Column;
//             UIComponentProfiles.View.style.position = Position.Relative;
//         }
//     }
// }