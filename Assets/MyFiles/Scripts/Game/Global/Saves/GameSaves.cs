using System;
using System.IO;
using UniRx;


public interface IGameSaves : IDisposable
{
    IReadOnlyReactiveDictionary<string, GameSave> Saves { get; }
    string ProfileId { get; }

    void LoadSaves(string profileId); // загрузить сохранения у конкретного профиля, без этого с сохранениями работать нельзя

    GameSave Create(GameSave gameSave);
    GameSave Create(GameSaveTypes type, string comment, string saveId);
    GameSave Update(GameSave save);
    void Delete(GameSave save);
}


/// <summary>
/// Сохранения у конкретного профиля
/// </summary>
public sealed class GameSaves : IGameSaves
{
    private static readonly Logger Logger = new("GameSaves", false);

    private readonly ReactiveDictionary<string, GameSave> _saves = new();
    public IReadOnlyReactiveDictionary<string, GameSave> Saves => _saves;

    public string ProfileId { get; private set; }


    // ---------------- lifecycle ----------------

    public void LoadSaves(string profileId)
    {
        ProfileId = profileId;

        _saves.Clear();

        string savesPath = DataPathManager.GameProfileSaves(profileId);

        if (!Directory.Exists(savesPath))
            return;

        foreach (string dir in Directory.GetDirectories(savesPath))
        {
            var data = SaveData.Load<GameSaveData>(dir, "Save");
            if (data == null)
                continue;

            var save = new GameSave();
            save.Load(data);

            _saves[save.SaveId.Value] = save;
        }

        Logger.Info($"Сохранения профиля {profileId} загружены");
    }

    public void Dispose()
    {
        _saves.Clear();
        ProfileId = null;
    }

    // ---------------- saves ----------------

    public GameSave Create(GameSaveTypes type, string comment, string saveId = "")
    {
        var gameSave = new GameSave(comment);
        if (saveId != "")
        {
            gameSave.SaveId.Value = saveId;
        }
        gameSave.ChangeSaveType(type);
        _saves[gameSave.SaveId.Value] = gameSave;

        gameSave = Create(gameSave);
        Logger.Info($"Создано сохранение {gameSave.SaveId.Value}");
        return gameSave;
    }

    public GameSave Create(GameSave gameSave)
    {
        EnsureOpened();
        SaveSave(gameSave);
        Logger.Info($"Создано сохранение {gameSave.SaveId.Value}");
        return gameSave;
    }

    public GameSave Update(GameSave save)
    {
        EnsureOpened();

        SaveSave(save);
        Logger.Debug($"Сохранение {save.SaveId.Value} обновлено");
        return save;
    }

    public void Delete(GameSave save)
    {
        EnsureOpened();

        string path = DataPathManager.GameProfileSave(ProfileId, save.SaveId.Value);

        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            _saves.Remove(save.SaveId.Value);

            Logger.Info($"Сохранение {save.SaveId.Value} удалено");
        }
        catch (Exception e)
        {
            Logger.Error($"Ошибка удаления сохранения {save.SaveId.Value}: {e.Message}");
        }
    }

    // ---------------- private ----------------

    private void SaveSave(GameSave save)
    {
        SaveData.Save(
            save.Save(),
            DataPathManager.GameProfileSave(ProfileId, save.SaveId.Value),
            "Save"
        );
    }

    private void EnsureOpened()
    {
        if (ProfileId == null)
            throw new InvalidOperationException("GameSaves не инициализирован. Вызови LoadSaves()");
    }
}
