using System;
using UnityEngine.UIElements;

namespace UITK
{
    // Слой интерфейса сессии (PauseScreen, InitialLoading, FocusTransition)
    public class SessionLayerComponent : UIComponent<Type>
    {
        public SessionLayerComponent(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            SetupLayerStyle(View);
        }

        public override void Render()
        {
            // Место для монтажа экранов паузы и экранов загрузки сессии
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
