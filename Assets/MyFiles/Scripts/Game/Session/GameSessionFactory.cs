// public class GameSessionFactory
// {
//     public GameSession CreateFromSave(GameProfile profile, GameSave save)
//     {
//         string sessionPath = CreateRuntimeFolder();
//         CopySaveToSession(profile, save, sessionPath);

//         GameSessionData data = LoadSessionData(sessionPath);

//         return new GameSession(profile, sessionPath, data);
//     }

//     public GameSession CreateNew(GameProfile profile, NewGameData newGameData)
//     {
//         string sessionPath = CreateRuntimeFolder();

//         GameSessionData data = BuildFromNewGameData(newGameData);

//         SaveData.Save(data, sessionPath, "GameSessionData");

//         return new GameSession(profile, sessionPath, data);
//     }
// }