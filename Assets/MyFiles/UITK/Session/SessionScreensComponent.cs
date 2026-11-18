using System;
using SessionSM;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    // Промежуточный компонент экранов сессии.
    // Монтируется только когда есть активная сессия, подписывается на CurrentState сессии
    // и рендерит актуальный экран (InitialLoading, EntitiesSync, PauseScreen).
    public class SessionScreensComponent : UIComponent<Type>
    {
        public SessionScreensComponent(Type props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _pauseScreen;
        private VisualTreeAsset _initialLoading;
        private VisualTreeAsset _entitiesSync;

        private IReadOnlyReactiveProperty<SessionStateBase> _sessionState;

        public override void Init()
        {
            _pauseScreen = LoadUITK("Session/PauseScreen/index.uxml");
            _initialLoading = LoadUITK("Session/InitialLoading/index.uxml");
            _entitiesSync = LoadUITK("Session/EntitiesSync/index.uxml");

            if (GlobalGame.Session?.SessionStateManager != null)
            {
                _sessionState = Use(GlobalGame.Session.SessionStateManager.CurrentState);
                // Debug.Log($"[SessionScreensComponent] Подписка на CurrentState сессии: {_sessionState?.Value}");
            }
        }

        public override void Render()
        {
            // Debug.Log($"[SessionScreensComponent] Рендер: {_sessionState?.Value}");
            var state = _sessionState?.Value;
            if (state == null) return;

            if (state is SessionInitialLoadingState)
            {
                var loading = GlobalGame.Session?.GameLoader?.LoadingProgress;
                Node<InitialLoadingComponent, GameLoadingProgress>(loading, _initialLoading, View, "initial_loading");
            }
            else if (state is SessionEntitiesSyncState)
            {
                Node<EntitiesSyncComponent, Type>(null, _entitiesSync, View, "entities_sync");
            }
            else if (state is SessionPauseScreenState)
            {
                Node<PauseScreenComponent, Type>(null, _pauseScreen, View, "pause_screen");
            }
        }
    }
}
