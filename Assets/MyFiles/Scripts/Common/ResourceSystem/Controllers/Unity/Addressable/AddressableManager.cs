using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace ResourceSystem
{
    public sealed class AddressableManager<T> : ResourceController<T>
    {
        protected override async Task<T> LoadAsync(string key)
        {
            return await Addressables.LoadAssetAsync<T>(key).Task;
        }

        protected override T Load(string key)
        {
            return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
        }

        protected override void UnloadInternal(ResourceAsset<T> asset)
        {
            Addressables.Release(asset.Resource);
        }
    }
}