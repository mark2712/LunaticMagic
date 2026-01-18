using System;
using UniRx;

public enum ProfileTypes
{
    User,   // обычный
    System  // системный (например для экрана загрузки)
}

public class GameProfile
{
    public readonly ReactiveProperty<string> ProfileId = new("");
    public readonly ReactiveProperty<string> Name = new("");
    public readonly ReactiveProperty<ProfileTypes> ProfileType = new(ProfileTypes.System);
    public readonly ReactiveProperty<DifficultyGame> Difficulty = new(DifficultyGame.Normal);
    public readonly ReactiveProperty<bool> IsArchive = new(false);
    public readonly ReactiveProperty<bool> IsLocked = new(false); // Рекомендуется только для UI

    public GameProfile(string name)
    {
        ProfileId.Value = $"Profile_{DateTime.Now:yyyyMMdd_HHmmss}";
        Name.Value = name;
        // VisibleName.Value = name;
    }

    public GameProfile ChangeProfileType(ProfileTypes type)
    {
        ProfileType.Value = type;
        return this;
    }

    public GameProfileData Save()
    {
        return new GameProfileData()
        {
            ProfileId = ProfileId.Value,
            Name = Name.Value,
            ProfileType = ProfileType.Value,
            Difficulty = Difficulty.Value,
            IsArchive = IsArchive.Value,
        };
    }

    public void Load(GameProfileData gameProfileData)
    {
        ProfileId.Value = gameProfileData.ProfileId;
        Name.Value = gameProfileData.Name;
        ProfileType.Value = gameProfileData.ProfileType;
        Difficulty.Value = gameProfileData.Difficulty;
        IsArchive.Value = gameProfileData.IsArchive;
    }
}

[Serializable]
public class GameProfileData
{
    public string ProfileId = GeneratorId.GenerateId("ProfileId");
    public string Name = "Profile name";
    public ProfileTypes ProfileType = ProfileTypes.System;
    public DifficultyGame Difficulty = DifficultyGame.Normal;
    public bool IsArchive = false;
}