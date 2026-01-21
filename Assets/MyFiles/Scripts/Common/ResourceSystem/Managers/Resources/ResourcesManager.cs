using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ResourceSystem
{
    public sealed class ResourcesManager<TResource>
    : ResourceManager<TResource>
    where TResource : UnityEngine.Object
    {
        protected override Task<TResource> LoadAsync(string key)
        {
            return Task.FromResult(Load(key));
        }

        protected override TResource Load(string key)
        {
            return Resources.Load<TResource>(key);
        }

        protected override void UnloadInternal(ResourceAsset<TResource> asset)
        {
            if (asset.Resource is GameObject)
            {
                // Õ≈À‹«ﬂ UnloadAsset ‰Îˇ GameObject
                return;
            }

            Resources.UnloadAsset(asset.Resource);
        }
    }

}