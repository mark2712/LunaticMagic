using System;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    // Системный слой приложения (Консоль ~, Debug-меню F12)
    public class SystemLayerComponent : UIComponent<Type>
    {
        public SystemLayerComponent(Type props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _debugMenu;
        private VisualTreeAsset _console;

        private IReadOnlyReactiveProperty<bool> _debugMenuState;
        private IReadOnlyReactiveProperty<bool> _consoleState;

        public override void Init()
        {
            SetupLayerStyle(View);

            _debugMenu = LoadUITK("Debug/Menu/index.uxml");
            _console = LoadUITK("Console/index.uxml");

            _debugMenuState = Use(GlobalGame.UIGlobalState.DebugMenu);
            _consoleState = Use(GlobalGame.UIGlobalState.Console);
        }

        public override void Render()
        {
            if (_debugMenuState.Value)
            {
                Node<DebugMenuComponent, Type>(null, _debugMenu, View, "debug_menu");
            }

            if (_consoleState.Value)
            {
                Node<ConsoleComponent, Type>(null, _console, View, "console");
            }
        }

        private static void SetupLayerStyle(VisualElement view)
        {
            view.style.position = Position.Absolute;
            view.style.left = 0;
            view.style.top = 0;
            view.style.right = 0;
            view.style.bottom = 0;
            view.pickingMode = PickingMode.Ignore;
        }
    }
}
