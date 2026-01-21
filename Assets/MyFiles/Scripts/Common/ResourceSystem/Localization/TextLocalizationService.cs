using System;
using System.Collections.Generic;


namespace ResourceSystem
{
    public sealed class TextLocalizationService
    {
        public readonly IResourceManager<Dictionary<string, string>> Manager = new FileLocalizationManager();

        public IResourceBinding<string> Bind(string resourceKey, string dictKey)
        {
            var resource = Manager.Bind(resourceKey);
            return new TextLocalizationBinding(resource, dictKey);
        }
    }
}