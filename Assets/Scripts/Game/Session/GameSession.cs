// Управление жизненным циклом игровой сессии, ее подсистемами и изолированной песочницей сохранения.
// Роль в проекте: центральный координатор сессии, объединяющий время (LocalTime), паузу (SessionPauseController),
// звук (SessionSoundController) и мир сущностей (EntitiesManager).

using System;
using System.IO;
using Entities;
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
    CursorState CursorState { get; }
    GameLoader GameLoader { get; }
    InputRouterGameplay InputRouterGameplay { get; }
    InputRouterPause InputRouterPause { get; }
    ISessionSoundController SessionSoundController { get; } // Реагирует на паузу и управляет 3 слотами музыки
    SessionStateManager SessionStateManager { get; } // Управляет состояниями сессии (InitialLoading, EntitiesSync, PauseScreen, Gameplay)

    void Init(NewGame.INewGameSessionChooser newGame);
    ReactiveProperty<bool> IsGameplay { get; } // Флаг: активный геймплей
    ReactiveProperty<bool> IsSessionInitLoading { get; } // Флаг: инициализация сессии. Важно: становится false только после полной загрузки, тоесть ПОЗЖЕ ВСЕХ флагов (включая IsGameplay, который становится true раньше)
    ReactiveProperty<bool> IsEntitiesSync { get; } // Флаг: синхронизация сущностей

    // Создает точку сохранения и упаковывает файлы песочницы сессии в архив профиля.
    void CreateSave(GameSaveTypes type, string comment);

    void FixedUpdate();
    void Update();
    void LateUpdate();

    void SetDifficulty(DifficultyGame difficulty); // Задает сложность игры для активного профиля.
    void SetProfileName(string name);
}

public class GameSession : IGameSession
{
    public string SessionId { get; private set; } = GeneratorId.GenerateId("Session");
    public string SessionPath { get; private set; }
    public string SavePath { get; private set; }
    public GameProfile Profile { get; private set; }

    public CursorState CursorState { get; }
    public GameLoader GameLoader { get; private set; }

    public InputRouterGameplay InputRouterGameplay { get; } = new();
    public InputRouterPause InputRouterPause { get; } = new();

    public ReactiveProperty<bool> IsGameplay { get; } = new(false);
    public ReactiveProperty<bool> IsSessionInitLoading { get; } = new(true);
    public ReactiveProperty<bool> IsEntitiesSync { get; } = new(false);

    public ILocalGameTime LocalTime { get; } = new LocalGameTime();
    public ISessionSoundController SessionSoundController { get; } = new SessionSoundController();

    public SessionStateManager SessionStateManager { get; }

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

        SessionStateManager = new SessionStateManager(IsSessionInitLoading, IsEntitiesSync, IsGameplay);
        CursorState = new CursorState(SessionStateManager);
        GameLoader = new GameLoader();
    }

    public void Init(NewGame.INewGameSessionChooser newGame)
    {
        // 3. Загружаем данные сессии из рантайм-папки
        GameSessionData RuntimeGameSessionData = SaveData.Load<GameSessionData>(SessionPath, "GameSessionData") ?? new();
        Load(RuntimeGameSessionData);

        // 4. Инициализируем каналы ввода
        InputRouterGameplay.Init(IsGameplay);
        InputRouterPause.Init(IsGameplay);
        
        GlobalGame.Session.InputRouterGameplay.GetButton(InputLayer.Inputs.E).OnDown += _ => { GlobalGame.Session.IsGameplay.Value = false; };
        GlobalGame.Session.InputRouterPause.GetButton(InputLayer.Inputs.E).OnUp += _ => { GlobalGame.Session.IsGameplay.Value = true; };

        // 5. Запуск процесса загрузки и синхронизации
        GameLoader.Start(newGame, this);
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
        if (IsGameplay.Value)
        {
            LocalTime.FixedUpdate(Time.fixedDeltaTime);
            EntitiesManager.FixedUpdate();
        }
    }

    public void Update()
    {
        if (!IsGameplay.Value)
        {
            LocalTime.Update(Time.unscaledDeltaTime);
            EntitiesManager.PauseUpdate();
        }
        else
        {
            EntitiesManager.Update();
        }
    }

    public void LateUpdate()
    {
        if (IsGameplay.Value)
        {
            EntitiesManager.LateUpdate();
        }
    }

    public void Dispose()
    {
        GameLoader?.Dispose();
        CursorState.Dispose();
        EntitiesManager.Dispose();
        SessionSoundController.Dispose();
        LocalTime.Dispose();
        InputRouterGameplay.Dispose();
        InputRouterPause.Dispose();
        IsGameplay.Dispose();
        IsSessionInitLoading.Dispose();
        IsEntitiesSync.Dispose();
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

    // --- Работа с профилем ---
    public void SetDifficulty(DifficultyGame difficulty)
    {
        if (Profile.Difficulty.Value == difficulty) return;
        Profile.Difficulty.Value = difficulty;
        ProfileUpdate(Profile);
    }

    public void SetProfileName(string name)
    {
        if (Profile.Name.Value == name) return;
        Profile.Name.Value = name;
        ProfileUpdate(Profile);
    }

    public void ProfileUpdate(GameProfile Profile)
    {
        using var profilesBinding = ResourceManager.Profiles.Bind("profiles"); // загрузить профили
        var profiles = profilesBinding.Resource;
        profiles.Update(Profile);
    }


    // --- Сериализация ---
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
