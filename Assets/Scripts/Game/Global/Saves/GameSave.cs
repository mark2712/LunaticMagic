using System;
using UniRx;

public enum GameSaveTypes
{
    Common, // Обычное
    Quick, // Быстрое
    Auto, // Автоматическое
    System // Системное
}

public class GameSave
{
    public readonly ReactiveProperty<string> SaveId = new(GenerateSaveId());
    public readonly ReactiveProperty<DateTime> CreatedAt = new(DateTime.Now);
    public readonly ReactiveProperty<string> Comment = new("");
    public readonly ReactiveProperty<GameSaveTypes> SaveType = new(GameSaveTypes.System);
    public readonly ReactiveProperty<bool> IsArchive = new(false);


    public GameSave(string comment = "")
    {
        SaveId.Value = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}";
        CreatedAt.Value = DateTime.Now;
        Comment.Value = comment;
    }

    public static string GenerateSaveId()
    {
        return $"{GeneratorId.GenerateId("Save")}_{DateTime.Now:yyyyMMdd_HHmmss}";
    }

    public GameSave ChangeSaveType(GameSaveTypes saveType)
    {
        SaveType.Value = saveType;
        return this;
    }

    public GameSaveData Save()
    {
        return new GameSaveData
        {
            SaveId = SaveId.Value,
            CreatedAt = SaveData.Date(CreatedAt.Value),
            Comment = Comment.Value,
            SaveType = SaveType.Value,
            IsArchive = IsArchive.Value,
        };
    }

    public void Load(GameSaveData data)
    {
        SaveId.Value = data.SaveId;
        CreatedAt.Value = SaveData.Date(data.CreatedAt);
        Comment.Value = data.Comment;
        SaveType.Value = data.SaveType;
        IsArchive.Value = data.IsArchive;
    }
}

[Serializable]
public class GameSaveData
{
    public string SaveId = GameSave.GenerateSaveId();
    public string CreatedAt = SaveData.Date(DateTime.Now);
    public string Comment = "";
    public GameSaveTypes SaveType = GameSaveTypes.System;
    public bool IsArchive = false;
}