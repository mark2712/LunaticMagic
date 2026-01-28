using System;
using InputLayer;
using UnityEngine;

public static class GlobalGame
{
    public static BootstrapGlobalGame Bootstrap;
    public static readonly UITK.UIController UIController = new(); // отвечает за рендер компонентов (в LateUpdate)
    public static readonly UITK.UIManager UIManager = new(); // отвечает создание CanvasPrefab и Root компонент
    public static UI.UIGlobalState UIGlobalState = new(); // простое состояние UI компонентов для тестирования

    public static IInputController InputController { get; private set; }

    public static GlobalGameSettings Settings = new();
    public static IGameProfiles Profiles = new GameProfiles();
    public static GameSession Session;

    static void GlobalGameInit()
    {
        Load();
        InputController = new InputController();

        Profiles.LoadProfiles(true); // если флаг true то будут созданы системные профили
        // Session = new(Profiles.Profiles[SystemProfileIds.SystemDebug]);
        Session = new(Profiles.Profiles["test1_ff28c2e80cd9"]);
        Profiles.Dispose();

        Save();
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
