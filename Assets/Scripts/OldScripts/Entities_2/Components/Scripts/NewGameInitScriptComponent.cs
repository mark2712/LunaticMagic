using UnityEngine;

namespace Entities_2
{
    public class NewGameInitScriptComponent : EntityComponentScript
    {
        public override void Execute()
        {
            Debug.Log("NewGame ТЕСТОВЫЙ скрипт, выполняется 1 раз");
            GlobalGame.UIGlobalState.OpenDebugMenu();
        }
    }
}