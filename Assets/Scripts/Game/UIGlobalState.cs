using UnityEngine;
using UniRx;

// <summary>
// Это не весь ui а только системный слой. Независим от геймплея и сессии.
// </summary>
public class UIGlobalState
{
    public readonly ReactiveProperty<bool> Console = new(false);
    public readonly ReactiveProperty<bool> DebugMenu = new(false);

    public SpawnVrm SpawnVrm = new();

    public void Init()
    {
        GlobalGame.InputController.GetButton(InputLayer.Inputs.F12).OnDown += _ => OpenDebugMenu();
        GlobalGame.InputController.GetButton(InputLayer.Inputs.Console).OnDown += _ => OpenConsole();
    }

    public void OpenConsole()
    {
        Console.Value = !Console.Value;
    }

    public void OpenDebugMenu()
    {
        DebugMenu.Value = !DebugMenu.Value;
    }
}
