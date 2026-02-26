using System;
using InputLayer;
using UniRx;
using UnityEngine;

public static class GlobalGame
{
    public static BootstrapGlobalGame Bootstrap;
    public static readonly UITK.UIController UIController = new(); // отвечает за рендер компонентов (в LateUpdate)
    public static readonly UITK.UIManager UIManager = new(); // отвечает создание CanvasPrefab и Root компонент
    public static UIGlobalState UIGlobalState = new(); // простое состояние UI компонентов для тестирования

    public static IInputController InputController { get; private set; }

    public static GlobalGameSettings Settings = new();
    // public static IGameProfiles Profiles = new GameProfiles();
    public static GameSession Session;
    public static ReactiveProperty<string> SessionProfuleId = new("");

    static void GlobalGameInit()
    {
        Load();
        InputController = new InputController();

        using var profilesBinding = ResourceManager.Profiles.Bind("profiles"); // загрузить профили
        var profiles = profilesBinding.Resource;
        // InitGameSession(profiles.Profiles[SystemProfileIds.SystemMainMenu]);
        InitGameSession(profiles.Profiles["test1_ff28c2e80cd9"]);

        Save();
    }

    public static void InitGameSession(GameProfile gameProfile)
    {
        ChangeSession(gameProfile, new NewGame.None(), null, true);
    }

    public static void NewGameSession(GameProfile gameProfile, NewGame.INewGameDataBuilder newGame)
    {
        ChangeSession(gameProfile, newGame, null);
    }

    public static void LoadGameSession(GameProfile gameProfile, GameSave gameSave)
    {
        ChangeSession(gameProfile, new NewGame.None(), gameSave);
    }

    private static void ChangeSession(GameProfile gameProfile, NewGame.INewGameDataBuilder newGame, GameSave gameSave = null, bool init = false)
    {
        Session?.Dispose();
        Session = new(gameProfile, gameSave);
        SessionProfuleId.Value = gameProfile.ProfileId.Value;
        if (init)
        {
            Session.Init(newGame);
            Debug.Log($"Сессия загружена {SessionProfuleId}");
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateAndInitializeBootstrap()
    {
        if (Bootstrap != null) return;
        GlobalGameInit();

        GameObject bootstrapGameObject = new("BootstrapGlobalGame");
        bootstrapGameObject.AddComponent<BootstrapGlobalGame>(); // AddComponent вызовет Awake, который присвоит GlobalGame.Bootstrap = this
    }

    public static void Load()
    {
        GlobalGameData globalGameData = SaveData.Load<GlobalGameData>(DataPathManager.GlobalSaves, "GlobalGameData");
        if (globalGameData != null)
        {
            Settings = globalGameData.GlobalGameSettings;
            ResourceManager.Localization.SetLanguage(globalGameData.Language);
        }
    }

    public static void Save()
    {
        GlobalGameData globalGameData = new()
        {
            GlobalGameSettings = Settings,
            Language = ResourceManager.Localization.Language.Value,
        };
        SaveData.Save(globalGameData, DataPathManager.GlobalSaves, "GlobalGameData");
    }
}


[Serializable]
public class GlobalGameData
{
    public GlobalGameSettings GlobalGameSettings = new();
    public string Language = ResourceSystem.ILocalization.BaseLanguage;
}

