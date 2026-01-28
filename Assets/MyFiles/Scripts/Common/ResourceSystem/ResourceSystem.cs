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

    public interface IResourceController<TResource>
    {
        Task<IResourceBinding<TResource>> BindAsync(string key);
        IResourceBinding<TResource> Bind(string key);
        IReadOnlyReactiveDictionary<string, ResourceAsset<TResource>> Assets { get; }
        void Clear(DateTime lastUseTime);
    }
}

public static class ResourceManager
{
    public static ResourceScopeManager ScopeManager = new();

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


// public static IResourceController<VisualTreeAsset> ResourcesVisualTreeAsset = new ResourcesManager<VisualTreeAsset>();
// public static IResourceController<Vrm10Instance> AddressableVRM = new VrmAddressableManager<Vrm10Instance>();
// public static IResourceController<Vrm10Instance> ResourcesVRM = new VrmResourcesManager<Vrm10Instance>();
// public static IResourceController<StyleSheet> AddressableStyleSheet = new AddressableManager<StyleSheet>();
