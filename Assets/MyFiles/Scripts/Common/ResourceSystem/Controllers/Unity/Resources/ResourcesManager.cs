using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ResourceSystem
{
    public sealed class ResourcesManager<TResource> : ResourceController<TResource> where TResource : UnityEngine.Object
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
                return;
            }

            Resources.UnloadAsset(asset.Resource);
        }
    }

}