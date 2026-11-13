using System;
using UnityEngine.UIElements;

namespace UITK
{
    public class UIComponentRoot : UIComponent<Type>
    {
        public UIComponentRoot(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
        }

        public override void Render()
        {
            RootVisualElementStyles(View);

            // 1. Игровой UI (сущности)
            Node<GameplayLayerComponent, Type>(null, null, View, "layer_gameplay");

            // 2. Сессионный UI (пауза, загрузка)
            Node<SessionLayerComponent, Type>(null, null, View, "layer_session");

            // 3. Системный UI (консоль, дебаг-меню)
            Node<SystemLayerComponent, Type>(null, null, View, "layer_system");
        }

        private void RootVisualElementStyles(VisualElement visualElement)
        {
            visualElement.style.flexGrow = 1;
            visualElement.style.flexDirection = FlexDirection.Column;
            visualElement.style.position = Position.Relative;
        }
    }
}
