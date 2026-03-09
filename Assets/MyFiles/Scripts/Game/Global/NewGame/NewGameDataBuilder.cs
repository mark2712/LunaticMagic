using Entities;
using UnityEngine;

namespace NewGame
{
    public interface INewGameChooser
    {
        void Execute(EntitiesManager EntitiesManager);
    }

    public class Save : INewGameChooser
    {
        public void Execute(EntitiesManager EntitiesManager) { }
    }

    public class MainMenu : INewGameChooser
    {
        public void Execute(EntitiesManager EntitiesManager)
        {
            IEntity entity = EntitiesManager.GetEntity("MainMenuInitScript");
        }
    }

    public class NewGame : INewGameChooser
    {
        public void Execute(EntitiesManager EntitiesManager)
        {
            IEntity entity = EntitiesManager.GetEntity("NewGameInitScript");
        }
    }
}



// namespace NewGame
// {
//     public interface INewGameDataBuilder
//     {
//         GameSessionData Build(); // выполняется ДО Load(RuntimeGameSessionData)
//     }

//     public class Save : INewGameDataBuilder
//     {
//         public GameSessionData Build()
//         {
//             Debug.LogWarning("INewGameDataBuilder не должен выполнятся у загружаемого сохранения");
//             return gameSessionData;
//         }
//     }

//     public class MainMenu : INewGameDataBuilder
//     {
//         public GameSessionData Build()
//         {
//             gameSessionData.StoryStateData.MainMenuEntitySpawn = true;
//             return gameSessionData;
//         }
//     }

//     public class NewGame : INewGameDataBuilder
//     {
//         public GameSessionData Build()
//         {
//             gameSessionData.StoryStateData.GameEntitySpawn = true;
//             return gameSessionData;
//         }
//     }
// }
