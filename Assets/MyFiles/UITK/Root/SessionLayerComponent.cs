using System;
using Game.Session.Pause;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    // Слой интерфейса сессии (PauseScreen, InitialLoading, EntitiesSync)
    public class SessionLayerComponent : UIComponent<Type>
    {
        public SessionLayerComponent(Type props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _pauseScreen;
        private VisualTreeAsset _initialLoading;
        private VisualTreeAsset _entitiesSync;

        private IReadOnlyReactiveProperty<SessionState> _sessionState;

        public override void Init()
        {
            SetupLayerStyle(View);

            _pauseScreen = LoadUITK("Session/PauseScreen/index.uxml");
            _initialLoading = LoadUITK("Session/InitialLoading/index.uxml");
            _entitiesSync = LoadUITK("Session/EntitiesSync/index.uxml");

            Use(GlobalGame.SessionProfuleId);
            BindSessionState();
        }

        private void BindSessionState()
        {
            if (GlobalGame.Session?.SessionStateManager != null)
            {
                _sessionState = Use(GlobalGame.Session.SessionStateManager.CurrentState);
            }
        }

        public override void Render()
        {
            BindSessionState();

            var state = _sessionState?.Value;
            if (state == null) return;

            if (state is SessionStateInitialLoading)
            {
                Node<InitialLoadingComponent, Type>(null, _initialLoading, View, "initial_loading");
            }
            else if (state is SessionStateEntitiesSync)
            {
                Node<EntitiesSyncComponent, Type>(null, _entitiesSync, View, "entities_sync");
            }
            else if (state is SessionStatePauseScreen)
            {
                Node<PauseScreenComponent, Type>(null, _pauseScreen, View, "pause_screen");
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
