using System;
using UniRx;


public class GameProfile
{
    public readonly ReactiveProperty<string> ProfileId = new("");
    public readonly ReactiveProperty<DateTime> CreatedAt = new(DateTime.Now);
    public readonly ReactiveProperty<string> Name = new("");
    public readonly ReactiveProperty<ProfileTypes> ProfileType = new(ProfileTypes.System);
    public readonly ReactiveProperty<DifficultyGame> Difficulty = new(DifficultyGame.Normal);
    public readonly ReactiveProperty<bool> IsArchive = new(false);
    public readonly ReactiveProperty<bool> IsSession = new(false);

    public GameProfile(string name)
    {
        ProfileId.Value = GenerateProfileId();
        Name.Value = name;
    }

    public static string GenerateProfileId()
    {
        return $"{GeneratorId.GenerateId("Profile")}_{DateTime.Now:yyyyMMdd_HHmmss}";
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
            CreatedAt = SaveData.Date(CreatedAt.Value),
            Name = Name.Value,
            ProfileType = ProfileType.Value,
            Difficulty = Difficulty.Value,
            IsArchive = IsArchive.Value,
        };
    }

    public void Load(GameProfileData gameProfileData)
    {
        ProfileId.Value = gameProfileData.ProfileId;
        CreatedAt.Value = SaveData.Date(gameProfileData.CreatedAt);
        Name.Value = gameProfileData.Name;
        ProfileType.Value = gameProfileData.ProfileType;
        Difficulty.Value = gameProfileData.Difficulty;
        IsArchive.Value = gameProfileData.IsArchive;
    }
}

[Serializable]
public class GameProfileData
{
    public string ProfileId = GameProfile.GenerateProfileId();
    public string CreatedAt = SaveData.Date(DateTime.Now);
    public string Name = "Profile name";
    public ProfileTypes ProfileType = ProfileTypes.System;
    public DifficultyGame Difficulty = DifficultyGame.Normal;
    public bool IsArchive = false;
}