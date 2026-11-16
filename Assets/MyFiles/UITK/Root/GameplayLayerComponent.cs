using System;
using Game.Session.Pause;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    // Базовый слой игрового интерфейса (HUD сущностей, индикаторы, игровой мир)
    public class GameplayLayerComponent : UIComponent<Type>
    {
        public GameplayLayerComponent(Type props, string key = "0") : base(props, key) { }

        private VisualTreeAsset _gameScreen;
        private IReadOnlyReactiveProperty<SessionState> _sessionState;

        public override void Init()
        {
            SetupLayerStyle(View);
            _gameScreen = LoadUITK("Gameplay/GameScreen/index.uxml");

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
            if (state is SessionStateGameplay)
            {
                Node<GameScreenComponent, Type>(null, _gameScreen, View, "game_screen");
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
