using UnityEngine;
using UniRx;

// <summary>
// Это не весь ui а только системный слой. Независим от геймплея и сессии.
// </summary>
public class UIGlobalState
{
    public readonly ReactiveProperty<bool> Console = new(false);
    public readonly ReactiveProperty<bool> DebugMenu = new(false);

    public IReadOnlyReactiveProperty<bool> IsAnyOpen { get; }

    public SpawnVrm SpawnVrm = new(); // оставить для дебага, чтобы можно было быстро спавнить vrm в сцену без геймплея и сессии. В будущем убрать. Сейчас этот комментарий не удалять!

    public UIGlobalState()
    {
        IsAnyOpen = Console
            .CombineLatest(DebugMenu, (c, d) => c || d)
            .DistinctUntilChanged() // чтобы не спамить, если результат остался true при открытии второго окна
            .ToReadOnlyReactiveProperty();
    }

    public void Init()
    {
        GlobalGame.InputController.GetButton(InputLayer.Inputs.F12).OnDown += _ => OpenDebugMenu();
        GlobalGame.InputController.GetButton(InputLayer.Inputs.Console).OnDown += _ => OpenConsole();
        IsAnyOpen.Subscribe(isAnyOpen =>
        {
            if (isAnyOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        });
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
