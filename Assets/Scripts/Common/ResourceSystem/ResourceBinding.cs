namespace ResourceSystem
{
    public class ResourceBinding<TResource> : IResourceBinding<TResource>
    {
        public virtual TResource Resource { get; }
        public string Key { get; }

        private readonly ResourceController<TResource> _manager;
        private bool _disposed;

        public ResourceBinding(ResourceController<TResource> manager, ResourceAsset<TResource> asset)
        {
            _manager = manager;
            Key = asset.Key;
            Resource = asset.Resource;

            asset.RefCount++;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _manager.ReleaseInternal(Key);
        }
    }
}
