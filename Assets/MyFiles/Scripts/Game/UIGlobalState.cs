using UnityEngine;
using UniRx;

namespace UI
{
    public class UIGlobalState
    {
        public readonly ReactiveProperty<bool> MainMenu = new(false);
        public readonly ReactiveProperty<bool> Console = new(false);
        public readonly ReactiveProperty<bool> Loading = new(false);
        public readonly ReactiveProperty<bool> DebugMenu = new(false);

        public SpawnVrm SpawnVrm = new();

        public void Init()
        {
            GlobalGame.InputController.GetButton(InputLayer.Inputs.F12).OnDown += _ => Profiles();
        }

        public void Profiles()
        {
            DebugMenu.Value = !DebugMenu.Value;
            if (DebugMenu.Value)
            {
                // GlobalGame.Session.CreateSave(GameSaveTypes.Quick, "");
                GlobalGame.Profiles.LoadProfiles();
            }
            else
            {
                GlobalGame.Profiles.Dispose();
            }
            // Debug.Log($"F12 {DebugMenu.Value}");
        }
    }
}