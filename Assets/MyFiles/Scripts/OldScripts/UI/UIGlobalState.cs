using UnityEngine;
using UniRx;

namespace UICanvas
{
    public class UIGlobalState
    {
        public readonly ReactiveProperty<bool> MainMenu = new(false);
        public readonly ReactiveProperty<bool> Console = new(false);
        public readonly ReactiveProperty<bool> Loading = new(false);
        public readonly ReactiveProperty<bool> DebugMenu = new(true);

        public void Init()
        {
            GlobalGame.InputController.GetButton(InputLayer.Inputs.F12).OnDown += _ =>
            {
                DebugMenu.Value = !DebugMenu.Value;
                // Debug.Log($"F12 {DebugMenu.Value}");
            };
        }
    }
}