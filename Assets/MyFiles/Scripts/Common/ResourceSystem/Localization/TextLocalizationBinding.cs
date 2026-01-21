using System;
using System.Collections.Generic;


namespace ResourceSystem
{
    public sealed class TextLocalizationBinding : IResourceBinding<string>
    {
        public string Key { get; private set; }

        private readonly IResourceBinding<Dictionary<string, string>> _resource;
        private bool _disposed;

        public TextLocalizationBinding(IResourceBinding<Dictionary<string, string>> resource, string dictKey)
        {
            _resource = resource;
            Key = dictKey;
        }

        public string Resource
        {
            get
            {
                if (_resource.Resource.TryGetValue(Key, out var value))
                    return value;

                return null;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _resource.Dispose();
        }
    }
}