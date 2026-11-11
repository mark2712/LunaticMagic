using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using UniVRM10;
using ResourceSystem;

namespace ResourceSystem
{
    public interface IResourceBinding<TResource> : IDisposable
    {
        TResource Resource { get; }
        string Key { get; }
    }

    public interface IResourceController<TResource> : ICleanable
    {
        Task<IResourceBinding<TResource>> BindAsync(string key);
        IResourceBinding<TResource> Bind(string key);
        IReadOnlyReactiveDictionary<string, ResourceAsset<TResource>> Assets { get; }
    }
}

public static class ResourceManager
{
    public static readonly ClearManager ClearManager = new();

    // Профили
    public static IResourceController<IGameProfiles> Profiles = new ProfilesResourceController();

    // Localization
    public static ILocalization Localization { get; private set; } = new LocalizationManager();

    // GameObject
    public static IResourceController<GameObject> AddressableGameObject = new AddressableManager<GameObject>();
    public static IResourceController<GameObject> ResourcesGameObject = new ResourcesManager<GameObject>();

    // UITK
    public static IResourceController<VisualTreeAsset> AddressableVisualTreeAsset = new AddressableManager<VisualTreeAsset>();

    // VRM
    public static IResourceController<Vrm10Instance> FileVRM = new VrmFileManager();
}
