using ResourceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.InputSystem;


namespace ResourceSystem
{
    public abstract class ResourceController<T> : IResourceController<T>
    {
        protected readonly ReactiveDictionary<string, ResourceAsset<T>> _assets = new();
        public IReadOnlyReactiveDictionary<string, ResourceAsset<T>> Assets => _assets;

        private readonly Dictionary<string, Task<ResourceAsset<T>>> _loading = new();

        public async Task<IResourceBinding<T>> BindAsync(string key)
        {
            var asset = await GetOrLoadAsync(key);
            return new ResourceBinding<T>(this, asset);
        }

        public IResourceBinding<T> Bind(string key)
        {
            var asset = GetOrLoad(key);
            return new ResourceBinding<T>(this, asset);
        }

        // ---------------- loading ----------------

        protected async Task<ResourceAsset<T>> GetOrLoadAsync(string key)
        {
            if (_assets.TryGetValue(key, out var existing))
                return existing;

            if (_loading.TryGetValue(key, out var task))
                return await task;

            var loadTask = LoadInternalAsync(key);
            _loading[key] = loadTask;

            var asset = await loadTask;
            _loading.Remove(key);
            _assets[key] = asset;

            return asset;
        }

        protected ResourceAsset<T> GetOrLoad(string key)
        {
            if (_assets.TryGetValue(key, out var asset))
                return asset;

            asset = LoadInternal(key);
            _assets[key] = asset;
            return asset;
        }

        // ---------------- release ----------------

        internal void ReleaseInternal(string key)
        {
            if (_assets.TryGetValue(key, out var asset))
            {
                asset.RefCount--;
            }
        }

        // ---------------- clear ----------------

        public void Clear(DateTime lastUseTime)
        {
            foreach (var asset in _assets.Values.ToList())
            {
                if (asset.CanUnload && asset.LastUseTime.Value < lastUseTime)
                {
                    UnloadInternal(asset);
                    _assets.Remove(asset.Key);
                }
            }
        }

        // ---------------- hooks ----------------

        protected abstract Task<T> LoadAsync(string key);
        protected abstract T Load(string key);
        protected abstract void UnloadInternal(ResourceAsset<T> asset);

        private async Task<ResourceAsset<T>> LoadInternalAsync(string key)
        {
            var resource = await LoadAsync(key);
            return new ResourceAsset<T>(key, resource);
        }

        private ResourceAsset<T> LoadInternal(string key)
        {
            var resource = Load(key);
            return new ResourceAsset<T>(key, resource);
        }
    }
}
