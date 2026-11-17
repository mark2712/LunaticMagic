using System;
using UnityEngine.UIElements;

namespace UITK
{
    // Слой интерфейса сессии.
    // Слушает смену сессии через SessionProfileId и монтирует SessionScreensComponent только когда сессия активна.
    public class SessionLayerComponent : UIComponent<Type>
    {
        public SessionLayerComponent(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            SetupLayerStyle(View);
            Use(GlobalGame.SessionId);
        }

        public override void Render()
        {
            string SessionId = GlobalGame.SessionId?.Value;
            if (string.IsNullOrEmpty(SessionId) || GlobalGame.Session == null)
            {
                return;
            }

            Node<SessionScreensComponent, Type>(null, null, View, $"session_screens_{SessionId}");
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
