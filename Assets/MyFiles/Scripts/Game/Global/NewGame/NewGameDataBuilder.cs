using System;

namespace NewGame
{
    public interface INewGameDataBuilder
    {
        GameSessionData Build(GameSessionData gameSessionData);
    }

    public class None : INewGameDataBuilder
    {
        public GameSessionData Build(GameSessionData gameSessionData)
        {
            return gameSessionData;
        }
    }

    public class MainMenu : INewGameDataBuilder
    {
        public GameSessionData Build(GameSessionData gameSessionData)
        {
            gameSessionData.StoryStateData.MainMenuOpen = true;
            return gameSessionData;
        }
    }

    public class NewGame : INewGameDataBuilder
    {
        public GameSessionData Build(GameSessionData gameSessionData)
        {
            return gameSessionData;
        }
    }
}

// public enum NewGameTypes
// {
//     None,
//     MainMenu,
//     NewGame,
//     Level1, // технический выбор уровня
// }

// public class NewGameDataBuilder
// {
//     public GameSessionData Build(GameSessionData gameSessionData, NewGameTypes newGameType)
//     {
//         switch (newGameType)
//         {
//             case NewGameTypes.MainMenu:
//                 gameSessionData = MainMenu(gameSessionData);
//                 break;
//             case NewGameTypes.NewGame:
//                 break;
//             case NewGameTypes.Level1:
//                 break;
//             default:
//                 break;
//         }
//         return gameSessionData;
//     }

//     public GameSessionData MainMenu(GameSessionData gameSessionData)
//     {
//         gameSessionData.MainMenuOpen = true;
//         return gameSessionData;
//     }

//     public GameSessionData Level1(GameSessionData gameSessionData)
//     {
//         return gameSessionData;
//     }
// }




// [Serializable]
// public class NewGameData
// {
//     public bool MainMenuOpen = true;
// }

// public static class NewGameDataCreater
// {
//     public static NewGameData Level0()
//     {
//         return new NewGameData();
//     }
// }



// public interface INewGameDataCreater
// {
//     public NewGameData Create();
// }

// public class NewGameDataLevel0 : INewGameDataCreater
// {
//     public NewGameData Create()
//     {
//         return new NewGameData();
//     }
// }