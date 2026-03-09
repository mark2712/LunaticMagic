using System;
using System.IO;
using Entities;
using UniRx;
using UnityEngine;


/// <summary>
///  Init - Enter, start | Dispose - Exit, stop | остальное на строне паузы
/// </summary>
public interface IGameSession : IDisposable
{
    string SessionId { get; }
    GameProfile Profile { get; }

    void Init(NewGame.INewGameChooser newGame); // Enter, start
    // за паузу отвечает пауза а не сессия
    // Dispose - это Exit, stop

    void CreateSave(GameSaveTypes type, string comment); // создаст новый GameSave GameSaves.Create() и сохранит данные в него

    void FixedUpdate();
    void Update();
    void LateUpdate();

    public void SetDifficulty(DifficultyGame difficulty);
}


public class GameSession : IGameSession
{
    public string SessionId { get; private set; } = GeneratorId.GenerateId("Session");
    public string SessionPath { get; private set; }
    public string SavePath { get; private set; }
    public GameProfile Profile { get; private set; }

    public LocalGameTime LocalTime = new();
    public Story.StoryState StoryState = new();
    public Story.QuestsManager QuestsManager = new();
    public PlayerState Player = new();
    public ICameras Cameras = new Cameras();
    // public NpcAi NpcAi = new();

    public EntitiesManager EntitiesManager = new();


    public GameSession(GameProfile gameProfile, GameSave gameSave = null)
    {
        Profile = gameProfile;
        Profile.IsSession.Value = true;

        // 1. Создаём папку сессии
        SessionPath = Path.Combine(DataPathManager.Sessions, SessionId);
        Directory.CreateDirectory(SessionPath);
        CleanupSessionsDirectory(SessionPath); // удалить все папки из Sessions кроме папки с текущей сессией

        // 2. Если есть сейв — копируем всё его содержимое в сессию
        if (gameSave != null)
        {
            SavePath = DataPathManager.GameProfileSave(Profile.ProfileId.Value, gameSave.SaveId.Value);
            DataPathManager.CopyDirectory(SavePath, SessionPath);
        }
    }

    public void Init(NewGame.INewGameChooser newGame)
    {
        // 3. Загружаем данные сессии из рантайм-папки
        GameSessionData RuntimeGameSessionData = SaveData.Load<GameSessionData>(SessionPath, "GameSessionData") ?? new();
        Load(RuntimeGameSessionData);

        newGame.Execute(EntitiesManager);
        // EntitiesManager.InitAllActiveEntities();
        // EntitiesManager.StartAll();
    }

    public void CreateSave(GameSaveTypes type, string comment)
    {
        // 1. Остановить сессию / паузу | (если нужно — снаружи)

        // 2. Создать сохранение
        GameSave gameSave = new();
        using var profilesBinding = ResourceManager.Profiles.Bind("profiles"); // загрузить профили
        var profiles = profilesBinding.Resource;
        IGameSaves gameSaves = profiles.LoadSaves(Profile.ProfileId.Value); // загрузить сохранения профиля
        gameSave = gameSaves.Create(gameSave); // создать папку для сохранения сессии
        SavePath = DataPathManager.GameProfileSave(Profile.ProfileId.Value, gameSave.SaveId.Value); // новая папа с сохранением

        // 3. Записать данные в рантайм папку
        GameSessionData gameSessionData = Save();
        SaveData.Save(gameSessionData, SessionPath, "GameSessionData");

        // 4. Дать другим системам сохраниться
        // Entities.SaveAll(SessionPath);
        // World.Save(SessionPath);

        // 5. Скопировать рантайм → сейв
        DataPathManager.CopyDirectory(SessionPath, SavePath);
    }

    public void FixedUpdate()
    {
        EntitiesManager.FixedUpdate();
    }

    public void Update()
    {
        if (LocalTime.IsPaused.Value)
        {
            EntitiesManager.PauseUpdate();
        }
        else
        {
            LocalTime.Tick(Time.deltaTime);
            EntitiesManager.Update();
        }
    }

    public void LateUpdate()
    {
        EntitiesManager.LateUpdate();
    }

    public void Dispose()
    {
        EntitiesManager.Dispose();
        Profile.IsSession.Value = false;
    }

    private void CleanupSessionsDirectory(string sessionPath)
    {
        string sessionsRoot = DataPathManager.Sessions;

        if (!Directory.Exists(sessionsRoot))
            return;

        foreach (var dir in Directory.GetDirectories(sessionsRoot))
        {
            // оставляем только текущую сессию
            if (Path.GetFullPath(dir) == Path.GetFullPath(sessionPath))
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

    public void SetDifficulty(DifficultyGame difficulty)
    {
        if (Profile.Difficulty.Value == difficulty) return;
        Profile.Difficulty.Value = difficulty;

        using var profilesBinding = ResourceManager.Profiles.Bind("profiles"); // загрузить профили
        var profiles = profilesBinding.Resource;
        profiles.Update(Profile);
    }

    public void SetName(string name)
    {
        if (Profile.Name.Value == name) return;
        Profile.Name.Value = name;

        using var profilesBinding = ResourceManager.Profiles.Bind("profiles"); // загрузить профили
        var profiles = profilesBinding.Resource;
        profiles.Update(Profile);
    }


    public void Load(GameSessionData data)
    {
        LocalTime.Load(data.LocalGameTimeData);
        StoryState.Load(data.StoryStateData);
        EntitiesManager.Load(data.EntitiesData);
    }

    public GameSessionData Save()
    {
        GameSessionData data = new()
        {
            LocalGameTimeData = LocalTime.Save(),
            StoryStateData = StoryState.Save(),
            EntitiesData = EntitiesManager.Save(),
        };
        return data;
    }
}


[Serializable]
public class GameSessionData
{
    public LocalGameTimeData LocalGameTimeData = new();
    public Story.StoryStateData StoryStateData = new();
    public EntitiesData EntitiesData = new();
}
