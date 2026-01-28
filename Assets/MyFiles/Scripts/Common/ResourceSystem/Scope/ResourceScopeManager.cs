using System;
using System.Collections.Generic;


namespace ResourceSystem
{
    public sealed class ResourceScopeManager
    {
        private readonly Dictionary<string, int> _pinCounts = new();

        public ResourceScope Create()
        {
            return new ResourceScope(this);
        }

        public void Pin(string key)
        {
            _pinCounts.TryGetValue(key, out var count);
            _pinCounts[key] = count + 1;
        }

        public void Unpin(string key)
        {
            if (!_pinCounts.TryGetValue(key, out var count))
                return;

            if (count <= 1)
            {
                _pinCounts.Remove(key);
            }
            else
            {
                _pinCounts[key] = count - 1;
            }
        }

        public bool IsPinned(string key) => _pinCounts.ContainsKey(key);
    }
}