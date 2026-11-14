// Управление жизненным циклом игровой сессии, ее подсистемами и изолированной песочницей сохранения.
// Роль в проекте: центральный координатор сессии, объединяющий время (LocalTime), паузу (SessionPauseController),
// звук (SessionSoundController) и мир сущностей (EntitiesManager).

using System;
using System.IO;
using Entities;
using Game.Session.Pause;
using Game.Session.Sound;
using LocalTime;
using UniRx;
using UnityEngine;

// Интерфейс активной игровой сессии. Определяет контракт доступа к состоянию сессии, времени, паузе, звуку и сохранению.
public interface IGameSession : IDisposable
{
    string SessionId { get; }
    GameProfile Profile { get; }
    ILocalGameTime LocalTime { get; }
    ISessionInputRouter InputRouter { get; } // маршрутизатор ввода для сессии
    ISessionPauseController SessionPauseController { get; } // Вся логика режимов паузы
    ISessionSoundController SessionSoundController { get; } // Реагирует на паузу и управляет 3 слотами музыки

    void Init(NewGame.INewGameSessionChooser newGame);
    IReadOnlyReactiveProperty<bool> IsPaused { get; } // Флаг: пауза или геймплей

    // Создает точку сохранения и упаковывает файлы песочницы сессии в архив профиля.
    void CreateSave(GameSaveTypes type, string comment);

    void FixedUpdate();
    void Update();
    void LateUpdate();

    // Задает сложность игры для активного профиля.
    void SetDifficulty(DifficultyGame difficulty);
}

public class GameSession : IGameSession
{
    public string SessionId { get; private set; } = GeneratorId.GenerateId("Session");
    public string SessionPath { get; private set; }
    public string SavePath { get; private set; }
    public GameProfile Profile { get; private set; }

    public ISessionInputRouter InputRouter { get; } = new SessionInputRouter(); // маршрутизатор ввода для сессии

    public ILocalGameTime LocalTime { get; } = new LocalGameTime();
    public ISessionPauseController SessionPauseController { get; } = new SessionPauseController();
    public ISessionSoundController SessionSoundController { get; } = new SessionSoundController();
    public IReadOnlyReactiveProperty<bool> IsPaused => SessionPauseController.IsPaused;

    public EntitiesManager EntitiesManager = new();
    public EntitiesLoader EntitiesLoader = new();


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

    public void Init(NewGame.INewGameSessionChooser newGame)
    {
        // 3. Загружаем данные сессии из рантайм-папки
        GameSessionData RuntimeGameSessionData = SaveData.Load<GameSessionData>(SessionPath, "GameSessionData") ?? new();
        Load(RuntimeGameSessionData);

        // EntitiesManager уже готов после Load, но нужно проверить все ли уникальные сущности загружены (например при новой игре)
        EntitiesLoader.RegisterMissingEntities(EntitiesManager);

        newGame.Execute(EntitiesManager);
        // EntitiesManager.InitAllActiveEntities();
        // EntitiesManager.StartAll();

        InputRouter.Init(IsPaused);
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
        // EntitiesManager.SaveAllEntities();

        // 5. Скопировать рантайм → сейв
        DataPathManager.CopyDirectory(SessionPath, SavePath);
    }

    public void FixedUpdate()
    {
        if (!IsPaused.Value)
        {
            LocalTime.FixedUpdate(Time.fixedDeltaTime);
            EntitiesManager.FixedUpdate();
        }
    }

    public void Update()
    {
        if (IsPaused.Value)
        {
            LocalTime.Update(Time.unscaledDeltaTime);
            SessionPauseController.Update();
            EntitiesManager.PauseUpdate();
        }
        else
        {
            EntitiesManager.Update();
        }
    }

    public void LateUpdate()
    {
        if (!IsPaused.Value)
        {
            EntitiesManager.LateUpdate();
        }
    }

    public void Dispose()
    {
        InputRouter.Dispose();
        SessionPauseController.Dispose();
        SessionSoundController.Dispose();
        LocalTime.Dispose();
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
        EntitiesManager.Load(data.EntitiesData);
        SessionSoundController.Load(data.SessionSoundData);
    }

    public GameSessionData Save()
    {
        GameSessionData data = new()
        {
            LocalGameTimeData = LocalTime.Save(),
            EntitiesData = EntitiesManager.Save(),
            SessionSoundData = SessionSoundController.Save(),
        };
        return data;
    }
}


// Сериализуемый контейнер полного состояния сессии для сохранения на диск.
[Serializable]
public class GameSessionData
{
    public LocalGameTimeData LocalGameTimeData = new();
    public EntitiesData EntitiesData = new();
    public SessionSoundData SessionSoundData = new();
}
