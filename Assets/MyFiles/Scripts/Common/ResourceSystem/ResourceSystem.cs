using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UniVRM10;
using System.Linq;
using System.Resources;


namespace ResourceSystem
{
    public class ResourceSystem
    {
        // GameObject
        public IResourceManager<GameObject> AddressableGameObject = new AddressableManager<GameObject>();
        public IResourceManager<GameObject> ResourcesGameObject = new ResourcesManager<GameObject>();

        // VisualTreeAsset
        public IResourceManager<VisualTreeAsset> AddressableVisualTreeAsset = new AddressableManager<VisualTreeAsset>();
        public IResourceManager<VisualTreeAsset> ResourcesVisualTreeAsset = new ResourcesManager<VisualTreeAsset>();

        // Localization
        public IResourceManager<Dictionary<string, string>> FileLocalization = new FileLocalizationManager();

        // VRM
        //public IResourceManager<Vrm10Instance> AddressableVRM = new VrmAddressableManager<Vrm10Instance>();
        //public IResourceManager<Vrm10Instance> ResourcesVRM = new VrmResourcesManager<Vrm10Instance>();
        public IResourceManager<Vrm10Instance> FileVRM = new VrmFileManager();
    }

    public interface IResourceBinding<TResource> : IDisposable
    {
        TResource Resource { get; }
        string Key { get; }
    }

    public interface IResourceManager<TResource>
    {
        Task<IResourceBinding<TResource>> BindAsync(string key);
        IResourceBinding<TResource> Bind(string key);
        IReadOnlyDictionary<string, ResourceAsset<TResource>> Assets { get; } // < ключ, ассет >
        void Clear(DateTime lastUseTime); // будут удалены только те ресурсы у которых LastUseTime < lastUseTime
    }

    public class ResourceAsset<TResource>
    {
        public string Key;
        public TResource Resource;
        public int RefCount;

        public bool CanUnload => RefCount == 0;

        public DateTime LastUseTime; // Для умной очистки
        public void UpdateLastUseTime()
        {
            LastUseTime = DateTime.UtcNow;
        }
    }

    public enum ResourceLoadStrategy
    {
        Resources,
        File,
        Addressable,
    }
}


