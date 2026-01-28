using System;
using System.IO;
using System.Linq;
using UniRx;


public interface IGameProfiles : IDisposable
{
    int MaxUserProfiles { get; set; }
    bool IsMaxUserProfiles();

    IReadOnlyReactiveDictionary<string, GameProfile> Profiles { get; }
    IReadOnlyReactiveDictionary<string, IGameSaves> GameProfilesSaves { get; }

    void LoadProfiles(bool isCreateInitProfiles = false); // загрузить все профили из папки с профилями, при первом запуске обязателен флаг true
    IGameSaves LoadSaves(string ProfileId); // сразу загружает и профили и сохранения для профиля ProfileId

    GameProfile Create(GameProfile gameProfile);
    GameProfile Create(string name, ProfileTypes type);
    GameProfile Create(string name, string profileId, ProfileTypes type);

    GameProfile Update(GameProfile profile);
    void Delete(GameProfile profile);

    void Archive(GameProfile profile);
    void Restore(GameProfile profile);
}


/// <summary>
/// LoadProfiles - открыть, Dispose - закрыть |
/// Сначала надо загрузить профили из папки LoadProfiles().
/// Далее загрузить LoadSaves(ProfileId) сохранения GameSaves у конкретного профиля.
/// И только потом можно работать с сохранениями (например создать сохранение).
/// В конце Dispose, так как профили нужны очень редко (только во время сохранения и иногда в меню паузы) их и сохранения надо очищать из памяти.
/// </summary>
public sealed partial class GameProfiles : IGameProfiles
{
    public int MaxUserProfiles { get; set; } = 100;

    private readonly ReactiveDictionary<string, GameProfile> _profiles = new();
    public IReadOnlyReactiveDictionary<string, GameProfile> Profiles => _profiles;

    private readonly ReactiveDictionary<string, IGameSaves> _gameProfilesSaves = new();
    public IReadOnlyReactiveDictionary<string, IGameSaves> GameProfilesSaves => _gameProfilesSaves;

    private static readonly Logger Logger = new("GameProfiles");


    // ---------------- profiles ----------------

    public bool IsMaxUserProfiles()
    {
        int userCount = _profiles.Values.Count(p => p.ProfileType.Value != ProfileTypes.System);
        return userCount >= MaxUserProfiles;
    }

    public GameProfile Update(GameProfile profile)
    {
        // if (profile.IsLocked.Value) return profile;
        SaveProfile(profile);
        return profile;
    }

    public void Delete(GameProfile profile)
    {
        if (profile.IsSession.Value) return;
        string profileId = profile.ProfileId.Value;
        DisposeGameSaves(profileId);
        string path = Path.Combine(DataPathManager.Profiles, profileId);
        TryDeleteDirectory(path);
        _profiles.Remove(profileId);
        Logger.Info($"Профиль {profileId} удалён");
    }

    // ---------------- archive ----------------

    public void Archive(GameProfile profile)
    {
        profile.IsArchive.Value = true;
        Logger.Info($"Профиль {profile.ProfileId.Value} архивирован");
    }

    public void Restore(GameProfile profile)
    {
        profile.IsArchive.Value = false;
        Logger.Info($"Профиль {profile.ProfileId.Value} восстановлен");
    }

    // ---------------- load ----------------

    public void LoadProfiles(bool isCreateInitProfiles = false)
    {
        DisposeAllGameSaves();
        _profiles.Clear();
        LoadFromDirectory(DataPathManager.Profiles);
        if (isCreateInitProfiles)
        {
            CreateInitProfiles();
        }
    }

    /// <summary>
    /// Улучшенный прокси метод для GameSaves.LoadSaves(ProfileId)
    /// </summary>
    public IGameSaves LoadSaves(string ProfileId)
    {
        IGameSaves gameSaves = GameProfilesSaves[ProfileId];
        gameSaves.LoadSaves(ProfileId);
        return gameSaves;
    }

    private void LoadFromDirectory(string basePath)
    {
        foreach (string dir in Directory.GetDirectories(basePath))
        {
            var data = SaveData.Load<GameProfileData>(dir, "Profile");
            if (data == null)
                continue;

            var profile = new GameProfile("");
            profile.Load(data);

            string profileId = profile.ProfileId.Value;

            _profiles[profileId] = profile;
            CreateGameSaves(profile);
        }
    }

    // ---------------- GameSaves lifecycle ----------------

    private void CreateGameSaves(GameProfile profile)
    {
        var saves = new GameSaves();
        saves.LoadSaves(profile.ProfileId.Value);

        _gameProfilesSaves[profile.ProfileId.Value] = saves;
    }

    private void DisposeGameSaves(string profileId)
    {
        if (_gameProfilesSaves.TryGetValue(profileId, out var saves))
        {
            saves.Dispose();
            _gameProfilesSaves.Remove(profileId);
        }
    }

    private void DisposeAllGameSaves()
    {
        foreach (var saves in _gameProfilesSaves.Values)
            saves.Dispose();

        _gameProfilesSaves.Clear();
    }

    // ---------------- filesystem ----------------

    private void SaveProfile(GameProfile profile)
    {
        SaveData.Save(
            profile.Save(),
            Path.Combine(DataPathManager.Profiles, profile.ProfileId.Value),
            "Profile"
        );
    }

    private static void TryDeleteDirectory(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }

    public void Dispose()
    {
        DisposeAllGameSaves();
        _profiles.Clear();
    }
}
