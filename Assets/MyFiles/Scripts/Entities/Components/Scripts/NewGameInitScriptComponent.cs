using UnityEngine;

namespace Entities
{
    public class NewGameInitScriptComponent : EntityComponentScript
    {
        protected override void Execute()
        {
            Debug.Log("NewGame ТЕСТОВЫЙ скрипт, выполняется 1 раз");
            GlobalGame.UIGlobalState.OpenDebugMenu();
        }
    }
}