using UnityEngine;
using UniRx;
using ResourceSystem;


public class UIGlobalState
{
    public readonly ReactiveProperty<bool> MainMenu = new(false);
    public readonly ReactiveProperty<bool> Console = new(false);
    public readonly ReactiveProperty<bool> Loading = new(false);
    public readonly ReactiveProperty<bool> DebugMenu = new(false);

    private static IResourceBinding<IGameProfiles> _profiles;
    public static IGameProfiles Profiles => _profiles.Resource;


    public SpawnVrm SpawnVrm = new();

    public void Init()
    {
        GlobalGame.InputController.GetButton(InputLayer.Inputs.F12).OnDown += _ => OpenDebugMenu();
    }

    public void OpenDebugMenu()
    {
        DebugMenu.Value = !DebugMenu.Value;
        if (DebugMenu.Value)
        {
            _profiles ??= ResourceManager.Profiles.Bind("profiles");
        }
        else
        {
            _profiles?.Dispose();
            _profiles = null;
        }
        // Debug.Log($"F12 {DebugMenu.Value}");
    }
}
