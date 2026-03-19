using UnityEngine;

namespace Entities
{
    public class MainMenuInitScriptComponent : EntityComponentScript
    {
        protected override void Execute()
        {
            Debug.Log("MainMenu ТЕСТОВЫЙ скрипт, выполняется 1 раз");
            GlobalGame.UIGlobalState.OpenDebugMenu();
        }
    }
}