using System;
using UnityEngine.UIElements;

namespace UITK
{
    // Базовый слой игрового интерфейса (HUD сущностей, индикаторы, игровой мир)
    public class GameplayLayerComponent : UIComponent<Type>
    {
        public GameplayLayerComponent(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            SetupLayerStyle(View);
        }

        public override void Render()
        {
            // Место для монтажа HUD и элементов интерфейса сущностей
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
