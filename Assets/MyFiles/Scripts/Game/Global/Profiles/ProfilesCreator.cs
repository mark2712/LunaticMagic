using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public sealed partial class GameProfiles : IGameProfiles
{
    private List<GameProfile> GetInitProfiles()
    {
        List<GameProfile> profiles = new()
        {
            ProfileFabric("System Main Menu",  ProfileTypes.System, DifficultyGame.Normal, SystemProfileIds.SystemMainMenu),
            ProfileFabric("System Load", ProfileTypes.System, DifficultyGame.Normal,SystemProfileIds.SystemLoad),
            ProfileFabric("System Debug", ProfileTypes.System, DifficultyGame.Normal, SystemProfileIds.SystemDebug),
        };
        return profiles;
    }

    private void CreateInitProfiles()
    {
        List<GameProfile> profiles = GetInitProfiles();
        foreach (var profile in profiles)
        {
            Create(profile);
        }
    }

    private GameProfile ProfileFabric(string name, ProfileTypes type, DifficultyGame difficulty, string profileId = null)
    {
        var profile = new GameProfile(name).ChangeProfileType(type);
        if (profileId != null)
        {
            profile.ProfileId.Value = profileId;
        }
        profile.Difficulty.Value = difficulty;
        return profile;
    }

    public GameProfile Create(string name, ProfileTypes type, DifficultyGame difficulty, string profileId = null)
    {
        return Create(ProfileFabric(name, type, difficulty, profileId));
    }

    public GameProfile Create(GameProfile profile)
    {
        string profileId = profile.ProfileId.Value;
        if (_profiles.ContainsKey(profileId))
        {
            if (profile.ProfileType.Value == ProfileTypes.System)
            {
                // Logger.Warning($"Системный профиль {profileId} уже существует, сейчас профиль обновится");
            }
            else
            {
                Logger.Warning($"Профиль {profileId} уже существует, сейчас профиль обновится");
            }
            return Update(_profiles[profileId]);
        }

        ProfileTypes type = profile.ProfileType.Value;
        if (type != ProfileTypes.System)
        {
            if (IsMaxUserProfiles())
            {
                Logger.Warning("Нельзя создать больше 100 пользовательских профилей");
                return null;
            }
        }

        _profiles[profileId] = profile;
        SaveProfile(profile);

        CreateGameSaves(profile);

        Logger.Info($"Создан профиль {profileId}");
        return profile;
    }
}