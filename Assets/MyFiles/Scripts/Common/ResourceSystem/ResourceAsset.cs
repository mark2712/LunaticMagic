using System;
using UniRx;


namespace ResourceSystem
{
    public interface IResourceAsset
    {
        string Key { get; }
        public bool CanUnloadForse { get; }
        public bool CanUnload { get; }
        public ReactiveProperty<DateTime> LastUseTime { get; }
        public int ScopePinCount { get; set; }
    }

    public class ResourceAsset<TResource> : IResourceAsset
    {
        public string Key { get; private set; }
        public readonly TResource Resource;

        public bool CanUnloadForse => RefCount == 0;
        public bool CanUnload => RefCount == 0 && !ResourceManager.ScopeManager.IsPinned(Key);
        // public bool CanUnload => RefCount == 0 && ScopePinCount == 0;

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

        public int ScopePinCount { get; set; }

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