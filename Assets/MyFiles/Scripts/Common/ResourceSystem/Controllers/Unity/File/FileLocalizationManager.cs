using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ResourceSystem
{
    public sealed class FileLocalizationManager : ResourceController<Dictionary<string, string>>
    {
        protected override Task<Dictionary<string, string>> LoadAsync(string key)
        {
            return Task.FromResult(Load(key));
        }

        protected override Dictionary<string, string> Load(string key)
        {
            var path = $"{DataPathManager.Localization}/{key}.json";
            // Debug.Log(path);

            if (!File.Exists(path))
                return new Dictionary<string, string>();

            var json = File.ReadAllText(path);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }

        protected override void UnloadInternal(ResourceAsset<Dictionary<string, string>> asset)
        {
            // не нужно, всё сделает GC
        }
    }
}