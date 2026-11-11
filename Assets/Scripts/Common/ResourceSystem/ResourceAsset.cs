using System;
using UniRx;


namespace ResourceSystem
{
    public interface IResourceAsset
    {
        string Key { get; }
        public bool CanUnload { get; }
        public ReactiveProperty<DateTime> LastUseTime { get; }
    }

    public class ResourceAsset<TResource> : IResourceAsset
    {
        public string Key { get; private set; }
        public readonly TResource Resource;

        public bool CanUnload => RefCount == 0;

        public ReactiveProperty<DateTime> LastUseTime { get; private set; } = new(DateTime.UtcNow);

        private int _refCount;
        public int RefCount
        {
            get => _refCount;
            set
            {
                if (_refCount == value)
                    return;

                _refCount = value;
                UpdateLastUseTime();
            }
        }

        public ResourceAsset(string key, TResource resource)
        {
            Key = key;
            Resource = resource;
        }

        public void UpdateLastUseTime()
        {
            LastUseTime.Value = DateTime.UtcNow;
        }
    }
}