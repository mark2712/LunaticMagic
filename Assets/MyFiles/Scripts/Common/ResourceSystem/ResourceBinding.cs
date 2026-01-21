namespace ResourceSystem
{
    public class ResourceBinding<TResource> : IResourceBinding<TResource>
    {
        public virtual TResource Resource { get; }
        public string Key { get; }

        private readonly ResourceManager<TResource> _manager;
        private bool _disposed;

        public ResourceBinding(ResourceManager<TResource> manager, ResourceAsset<TResource> asset)
        {
            _manager = manager;
            Key = asset.Key;
            Resource = asset.Resource;

            asset.RefCount++;
            asset.UpdateLastUseTime();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _manager.ReleaseInternal(Key);
        }
    }
}
