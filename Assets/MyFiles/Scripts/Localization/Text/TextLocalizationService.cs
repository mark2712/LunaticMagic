using System;
using System.Collections.Generic;
using System.IO;

public sealed class TextLocalizationService : ITextLocalization
{
    private readonly Dictionary<string, LocalizationModule> _modules = new();
    public IReadOnlyDictionary<string, LocalizationModule> Modules => _modules;

    public ILocalizationBinding Bind(string fullKey)
    {
        Acquire(fullKey);
        return new TextLocalizationBinding(this, fullKey);
    }

    public string Resolve(string fullKey)
    {
        Parse(fullKey, out var lang, out var modulePath, out var key);

        if (_modules.TryGetValue(BuildModuleKey(lang, modulePath), out var module))
        {
            module.UpdateLastUseTime();

            if (module.Data.TryGetValue(key, out var value))
                return value;
        }

        return null;
    }

    private void Acquire(string fullKey)
    {
        Parse(fullKey, out var lang, out var modulePath, out _);

        var id = BuildModuleKey(lang, modulePath);

        if (_modules.TryGetValue(id, out var module))
        {
            module.RefCount++;
            module.UpdateLastUseTime();
            return;
        }

        _modules[id] = LoadModule(lang, modulePath);
        _modules[id].RefCount = 1;
    }

    public void Release(string fullKey)
    {
        Parse(fullKey, out var lang, out var modulePath, out _);

        var id = BuildModuleKey(lang, modulePath);

        if (!_modules.TryGetValue(id, out var module))
            return;

        module.RefCount--;
        module.UpdateLastUseTime();
        // НЕ удаляем тут
    }

    // ---------------- loading ----------------

    private LocalizationModule LoadModule(string lang, string modulePath)
    {
        var path = Path.Combine(
            DataPathManager.Localization,
            lang,
            modulePath + ".json"
        );

        Dictionary<string, string> data;

        if (!File.Exists(path))
        {
            data = new Dictionary<string, string>();
        }
        else
        {
            var json = File.ReadAllText(path);
            data = Newtonsoft.Json.JsonConvert
                .DeserializeObject<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>();
        }

        return new LocalizationModule
        {
            Data = data,
            RefCount = 0,
            LastUseTime = DateTime.UtcNow
        };
    }

    // ---------------- parsing ----------------

    // fullKey: ru/ui/menu.text
    // modulePath: ui/menu
    // key: text
    private static void Parse(string fullKey, out string lang, out string modulePath, out string key)
    {
        int firstSlash = fullKey.IndexOf('/');
        int dot = fullKey.LastIndexOf('.');

        if (firstSlash <= 0 || dot <= firstSlash)
            throw new Exception($"Invalid localization key: {fullKey}");

        lang = fullKey.Substring(0, firstSlash);
        modulePath = fullKey.Substring(firstSlash + 1, dot - firstSlash - 1);
        key = fullKey.Substring(dot + 1);
    }

    public string BuildModuleKey(string lang, string modulePath)
    {
        return lang + "/" + modulePath;
    }

    public void Dispose()
    {
        _modules.Clear();
    }
}
