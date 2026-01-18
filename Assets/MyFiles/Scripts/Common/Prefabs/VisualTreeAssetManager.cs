using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;


/// <summary>
/// Загружает VisualTreeAsset из Resources.
/// Рекомендуется использовать метод LoadUITK который загружает сразу из Resources/Prefabs/UITK/
/// </summary>
public static class VisualTreeAssetManager
{
    // Кэш одиночных UXML
    private static readonly Dictionary<string, VisualTreeAsset> _cache = new();

    // Кэш папок
    private static readonly Dictionary<string, VisualTreeAsset[]> _folderCache = new();

    /// <summary>
    /// Загружает UXML из Resources/Prefabs/UITK/
    /// </summary>
    public static VisualTreeAsset LoadUITK(string path)
    {
        return Load($"Prefabs/UITK/{path}");
    }

    /// <summary>
    /// Загружает VisualTreeAsset из Resources
    /// </summary>
    public static VisualTreeAsset Load(string path)
    {
        if (_cache.TryGetValue(path, out var asset))
            return asset;

        asset = Resources.Load<VisualTreeAsset>(path);
        if (asset == null)
        {
            Debug.LogError($"VisualTreeAssetManager: не найден UXML по пути {path}");
            return null;
        }

        _cache[path] = asset;
        return asset;
    }

    /// <summary>
    /// Загружает все UXML из папки
    /// </summary>
    public static VisualTreeAsset[] LoadAll(string folder)
    {
        if (_folderCache.TryGetValue(folder, out var cached))
            return cached;

        var loaded = Resources.LoadAll<VisualTreeAsset>(folder);
        if (loaded == null || loaded.Length == 0)
        {
            Debug.LogWarning($"VisualTreeAssetManager: в папке {folder} нет UXML");
        }

        _folderCache[folder] = loaded;
        return loaded;
    }

    /// <summary>
    /// Создаёт VisualElement из UXML
    /// </summary>
    public static VisualElement Instantiate(string path)
    {
        var asset = Load(path);
        return asset?.CloneTree();
    }

    /// <summary>
    /// Случайный UXML из папки
    /// </summary>
    public static VisualElement InstantiateRandom(string folder)
    {
        var assets = LoadAll(folder);
        if (assets.Length == 0) return null;

        var asset = assets[Random.Range(0, assets.Length)];
        return asset.CloneTree();
    }

    /// <summary>
    /// Очистка кэша
    /// </summary>
    public static void ClearCache()
    {
        _cache.Clear();
        _folderCache.Clear();
    }
}
