using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public sealed partial class GameProfiles : IGameProfiles
{
    private List<GameProfile> GetInitProfiles()
    {
        List<GameProfile> profiles = new()
        {
            ProfileFabric("System Main Menu", SystemProfileIds.SystemMainMenu, ProfileTypes.System),
            ProfileFabric("System Load", SystemProfileIds.SystemLoad, ProfileTypes.System),
            ProfileFabric("System Debug", SystemProfileIds.SystemDebug, ProfileTypes.System),
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

    private GameProfile ProfileFabric(string name, string profileId, ProfileTypes type)
    {
        var profile = new GameProfile(name).ChangeProfileType(type);
        profile.ProfileId.Value = profileId;
        return profile;
    }

    public GameProfile Create(string name, ProfileTypes type)
    {
        string profileId = GeneratorId.GenerateId(name);
        return Create(name, profileId, type);
    }

    public GameProfile Create(string name, string profileId, ProfileTypes type)
    {
        return Create(ProfileFabric(name, profileId, type));
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