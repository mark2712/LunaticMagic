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
    /// Очистка кэша
    /// </summary>
    public static void ClearCache()
    {
        _cache.Clear();
    }
}
