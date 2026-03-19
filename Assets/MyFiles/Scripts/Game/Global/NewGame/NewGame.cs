using Entities;
using UnityEngine;

namespace NewGame
{
    public interface INewGameSessionChooser
    {
        void Execute(EntitiesManager EntitiesManager);
    }

    public class Save : INewGameSessionChooser
    {
        public void Execute(EntitiesManager EntitiesManager) { }
    }

    public class MainMenu : INewGameSessionChooser
    {
        public void Execute(EntitiesManager EntitiesManager)
        {
            IEntity entity = EntitiesManager.GetEntity("MainMenuInitScript");
            Debug.Log(entity);
            entity?.EntityRuntime?.Start();
        }
    }

    public class NewGame : INewGameSessionChooser
    {
        public void Execute(EntitiesManager EntitiesManager)
        {
            IEntity entity = EntitiesManager.GetEntity("NewGameInitScript");
            // IEntity entity = EntitiesManager.GetEntity("NewGameInitScript");
        }
    }
}
