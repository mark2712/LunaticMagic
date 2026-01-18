using System;
using System.IO;
using UnityEngine;


/// <summary>
///  Init - Enter, start | Dispose - Exit, stop | остальное на строне паузы
/// </summary>
public interface IGameSession : IDisposable
{
    string SessionId { get; }
    GameProfile Profile { get; }
    // GameSessionData RuntimeGameSessionData { get; } // берём из gameSave если он есть, если нет то тут всегда будут init данные

    void Init(); // Enter, start
    // Dispose - Exit, stop

    void CreateSave(GameSaveTypes type, string comment); // создаст новый GameSave GameSaves.Create() и сохранит данные в него

    void FixedUpdate();
    void Update();
    void LateUpdate();
}


public class GameSession : IGameSession
{
    public string SessionId { get; private set; } = GeneratorId.GenerateId("Session");
    public string SessionPath { get; private set; }
    public string SavePath { get; private set; }
    public GameProfile Profile { get; private set; }
    // public GameSessionData RuntimeGameSessionData { get; private set; } = new();
    public LocalGameTime LocalTime = new();

    public GameSession(GameProfile gameProfile, GameSave gameSave = null)
    {
        Profile = gameProfile;

        // 1. Создаём папку сессии
        SessionPath = Path.Combine(DataPathManager.Sessions, SessionId);
        Directory.CreateDirectory(SessionPath);

        // 2. Если есть сейв — копируем его содержимое в сессию
        if (gameSave != null)
        {
            SavePath = DataPathManager.GameProfileSave(gameProfile.ProfileId.Value, gameSave.SaveId.Value);
            DataPathManager.CopyDirectory(SavePath, SessionPath);
        }
    }

    public void Init()
    {
        // 3. Загружаем данные сессии из рантайм-папки
        GameSessionData RuntimeGameSessionData = SaveData.Load<GameSessionData>(SessionPath, "GameSessionData") ?? new();
        Load(RuntimeGameSessionData);

        // удалить все папки из Sessions кроме папки с текущей сессией
        CleanupSessionsDirectory();
    }

    public void CreateSave(GameSaveTypes type, string comment)
    {
        // 1. Остановить сессию / паузу | (если нужно — снаружи)

        // 2. Создать сохранение
        GameSave gameSave = new();
        GlobalGame.Profiles.LoadProfiles(); // загрузить профили
        IGameSaves gameSaves = GlobalGame.Profiles.LoadSaves(Profile.ProfileId.Value); // загрузить сохранения профиля
        gameSaves.Create(gameSave); // создать папку для сохранения сессии
        GlobalGame.Profiles.Dispose(); // закрыть профили (и их сохранения) чтобы не висели в памяти

        // 3. Записать данные в рантайм папку
        GameSessionData gameSessionData = Save();
        SaveData.Save(gameSessionData, "GameSessionData", SessionPath);
        // записать gameSessionData в файл SessionPath/GameSessionData.json
        // в будущем тут нужно будет не забыть Entities.SaveAll(SessionPath);
        // потом скопировать рантйм папку DataPathManager.CopyDirectory(SessionPath, savePath);

        // 4. Дать другим системам сохраниться
        // Entities.SaveAll(SessionPath);
        // World.Save(SessionPath);
        // Quests.Save(SessionPath);

        // 5. Скопировать рантайм → сейв
        DataPathManager.CopyDirectory(SessionPath, SavePath);
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        if (LocalTime.IsPaused.Value)
        {

        }
        else
        {
            LocalTime.Tick(Time.deltaTime);
        }
    }

    public void LateUpdate()
    {

    }


    public void Load(GameSessionData data)
    {
        LocalTime.Load(data.LocalGameTimeData);
    }

    public GameSessionData Save()
    {
        GameSessionData gameSessionData = new()
        {
            LocalGameTimeData = LocalTime.Save()
        };
        return gameSessionData;
    }


    public void Dispose()
    {

    }

    private void CleanupSessionsDirectory()
    {
        string sessionsRoot = DataPathManager.Sessions;

        if (!Directory.Exists(sessionsRoot))
            return;

        foreach (var dir in Directory.GetDirectories(sessionsRoot))
        {
            // оставляем только текущую сессию
            if (Path.GetFullPath(dir) == Path.GetFullPath(SessionPath))
                continue;

            try
            {
                Directory.Delete(dir, true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Не удалось удалить сессию {dir}: {e.Message}");
            }
        }
    }
}


[Serializable]
public class GameSessionData
{
    public LocalGameTimeData LocalGameTimeData = new();
}


// bool IsRunning { get; }
// void StartSession(); // запускает сессию
// void StopSession(); // пауза


// void CreateSession(GameProfile gameProfile, GameSave gameSave); // создает сессию (создаёт папку, копирует данные из gameSave)
// private void CreateSession(GameProfile gameProfile, GameSave gameSave = null) { }
