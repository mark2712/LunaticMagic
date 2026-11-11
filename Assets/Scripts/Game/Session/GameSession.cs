// Управление жизненным циклом игровой сессии, ее стейт-машиной и изолированной песочницей сохранения.
// Роль в проекте: центральный координатор сессии, объединяющий стейты (Loading, Gameplay, Pause), время, сущности и паузу.

using System;
using System.IO;
using Entities;
using Game.Session;
using Game.Session.Pause;
using LocalTime;
using UniRx;
using UnityEngine;

// Интерфейс активной игровой сессии. Определяет контракт доступа к состоянию сессии, времени, паузе и сохранению.
public interface IGameSession : IDisposable
{
    string SessionId { get; }
    GameProfile Profile { get; }
    IReadOnlyReactiveProperty<SessionState> State { get; }
    IPauseController PauseController { get; }
    LocalGameTime LocalTime { get; }

    // Переключает базовое состояние жизненного цикла сессии.
    void SetState(SessionState state);
    // Переводит сессию в состояние паузы (только из Gameplay).
    void Pause();
    // Возвращает сессию в режим геймплея (только из Pause).
    void Resume();
    // Инициализирует мир сессии и запускает стартовый сценарий.
    void Init(NewGame.INewGameSessionChooser newGame);

    // Создает точку сохранения и упаковывает файлы песочницы сессии в архив профиля.
    void CreateSave(GameSaveTypes type, string comment);

    // Физический тик симуляции сессии (выполняется строго в FixedUpdate).
    void FixedUpdate();
    // Кадровое обновление сессии (выполняется в Update).
    void Update();
    // Финальное кадровое обновление (выполняется в LateUpdate).
    void LateUpdate();

    // Задает сложность игры для активного профиля.
    void SetDifficulty(DifficultyGame difficulty);
}

// Реализация игровой сессии. Работает в изолированной папке Sessions/{SessionId}/ и управляет миром.
public class GameSession : IGameSession
{
    public string SessionId { get; private set; } = GeneratorId.GenerateId("Session");
    public string SessionPath { get; private set; }
    public string SavePath { get; private set; }
    public GameProfile Profile { get; private set; }

    // Реактивное свойство текущего состояния сессии (Loading, Gameplay, Pause).
    private readonly ReactiveProperty<SessionState> _state = new(SessionState.Loading);
    public IReadOnlyReactiveProperty<SessionState> State => _state;

    // Контроллер подстейтов паузы (меню паузы, консоль разработчика).
    public PauseController PauseController { get; } = new();
    IPauseController IGameSession.PauseController => PauseController;

    // Фасад управления временем сессии и календарем.
    public LocalGameTime LocalTime { get; } = new();
    public ICameras Cameras = new Cameras();

    public EntitiesManager EntitiesManager = new();
    public EntitiesLoader EntitiesLoader = new();

    // Конструктор сессии. Инициализирует рабочую директорию и принудительно замораживает время в состоянии Loading.
    public GameSession(GameProfile gameProfile, GameSave gameSave = null)
    {
        Profile = gameProfile;
        Profile.IsSession.Value = true;
        ApplyState(SessionState.Loading, force: true);

        // 1. Создаём папку сессии
        SessionPath = Path.Combine(DataPathManager.Sessions, SessionId);
        Directory.CreateDirectory(SessionPath);
        CleanupSessionsDirectory(SessionPath); // удалить все папки из Sessions кроме папки с текущей сессией

        // 2. Если передан сейв — копируем файлы сохранения в песочницу сессии
        if (gameSave != null)
        {
            SavePath = DataPathManager.GameProfileSave(Profile.ProfileId.Value, gameSave.SaveId.Value);
            DataPathManager.CopyDirectory(SavePath, SessionPath);
        }
    }

    // Задает новое целевое состояние сессии.
    public void SetState(SessionState newState)
    {
        ApplyState(newState, force: false);
    }

    // Применяет смену состояния с управлением паузой времени и сбросом подстейтов паузы.
    private void ApplyState(SessionState newState, bool force)
    {
        // if (!force && _state.Value == newState) return;

        // SessionState oldState = _state.Value;
        // _state.Value = newState;

        // switch (newState)
        // {
        //     case SessionState.Loading:
        //         AudioListener.pause = false;
        //         LocalTime.SetTimeScale(0f);
        //         break;
        //     case SessionState.Gameplay:
        //         AudioListener.pause = false;
        //         LocalTime.SetTimeScale(1f);
        //         break;
        //     case SessionState.Pause:
        //         AudioListener.pause = true;
        //         LocalTime.SetTimeScale(0f);
        //         break;
        // }

        // // При выходе из паузы в другое состояние сбрасываем активные подстейты паузы
        // if (oldState == SessionState.Pause && newState != SessionState.Pause)
        // {
        //     PauseController.Reset();
        // }
    }

    // Ставит игру на паузу (разрешено только из состояния геймплея).
    public void Pause()
    {
        if (_state.Value == SessionState.Gameplay)
        {
            SetState(SessionState.Pause);
        }
    }

    // Снимает игру с паузы и возвращает в геймплей (разрешено только из состояния паузы).
    public void Resume()
    {
        if (_state.Value == SessionState.Pause)
        {
            SetState(SessionState.Gameplay);
        }
    }

    // Инициализирует сущности сессии и передает управление стартовому сценарию.
    public void Init(NewGame.INewGameSessionChooser newGame)
    {
        SetState(SessionState.Loading);

        // Загружаем данные сессии из рабочей папки
        GameSessionData RuntimeGameSessionData = SaveData.Load<GameSessionData>(SessionPath, "GameSessionData") ?? new();
        Load(RuntimeGameSessionData);

        // Проверяем наличие всех уникальных сущностей
        EntitiesLoader.RegisterMissingEntities(EntitiesManager);

        newGame?.Execute(EntitiesManager);

        SetState(SessionState.Gameplay);
    }

    // Создает точку сохранения и копирует файлы рабочей сессии в постоянное хранилище.
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

    // Физический тик: продвижение времени мира и физики сущностей только в режиме геймплея.
    public void FixedUpdate()
    {
        if (_state.Value == SessionState.Gameplay)
        {
            LocalTime.FixedUpdate(Time.fixedDeltaTime);
            EntitiesManager.FixedUpdate();
        }
    }

    // Кадровый апдейт: в паузе обновляет оверлеи паузы, в геймплее — симуляцию сущностей.
    public void Update()
    {
        if (_state.Value == SessionState.Pause)
        {
            LocalTime.Update(Time.unscaledDeltaTime);
            PauseController.Update();
            EntitiesManager.PauseUpdate();
        }
        else if (_state.Value == SessionState.Gameplay)
        {
            EntitiesManager.Update();
        }
    }

    // Позднее кадровое обновление для камер и слежения в геймплее.
    public void LateUpdate()
    {
        if (_state.Value == SessionState.Gameplay)
        {
            EntitiesManager.LateUpdate();
        }
    }

    // Освобождает ресурсы сессии, сбрасывает контроллер времени движка и реактивные подписки.
    public void Dispose()
    {
        PauseController.Dispose();
        EntitiesManager.Dispose();
        LocalTime.Dispose();
        // AudioListener.pause = false;
        // LocalTime.SetTimeScale(1f);
        Profile.IsSession.Value = false;
        _state.Dispose();
    }

    // Очищает директорию сессий, удаляя временные папки завершенных сессий.
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

    // Обновляет уровень сложности в профиле игрока.
    public void SetDifficulty(DifficultyGame difficulty)
    {
        if (Profile.Difficulty.Value == difficulty) return;
        Profile.Difficulty.Value = difficulty;

        using var profilesBinding = ResourceManager.Profiles.Bind("profiles"); // загрузить профили
        var profiles = profilesBinding.Resource;
        profiles.Update(Profile);
    }

    // Обновляет имя персонажа в профиле игрока.
    public void SetName(string name)
    {
        if (Profile.Name.Value == name) return;
        Profile.Name.Value = name;

        using var profilesBinding = ResourceManager.Profiles.Bind("profiles"); // загрузить профили
        var profiles = profilesBinding.Resource;
        profiles.Update(Profile);
    }

    // Десериализует состояние времени, сюжета и сущностей.
    public void Load(GameSessionData data)
    {
        LocalTime.Load(data.LocalGameTimeData);
        EntitiesManager.Load(data.EntitiesData);
    }

    // Сериализует состояние времени, сюжета и сущностей в структуру сохранения.
    public GameSessionData Save()
    {
        GameSessionData data = new()
        {
            LocalGameTimeData = LocalTime.Save(),
            EntitiesData = EntitiesManager.Save(),
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
}
