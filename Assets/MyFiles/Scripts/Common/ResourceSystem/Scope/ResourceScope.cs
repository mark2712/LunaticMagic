using System;
using System.Collections.Generic;


namespace ResourceSystem
{
    public sealed class ResourceScope : IDisposable
    {
        private readonly ResourceScopeManager _manager;
        private readonly HashSet<string> _keys = new();

        internal ResourceScope(ResourceScopeManager manager)
        {
            _manager = manager;
        }

        public void AddKey(string key)
        {
            if (_keys.Add(key)) _manager.Pin(key);
        }

        public void RemoveKey(string key)
        {
            if (_keys.Remove(key)) _manager.Unpin(key);
        }

        public void Dispose()
        {
            foreach (var key in _keys) _manager.Unpin(key);
            _keys.Clear();
        }
    }
}