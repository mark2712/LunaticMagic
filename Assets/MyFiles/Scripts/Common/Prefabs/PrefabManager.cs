using UnityEngine;
using System.Collections.Generic;

public static class PrefabManager
{
    // Кэш для одиночных префабов
    private static readonly Dictionary<string, GameObject> _cache = new();

    /// <summary>
    /// Загружает префаб из Resources по пути.
    /// </summary>
    public static GameObject Load(string path)
    {
        if (_cache.TryGetValue(path, out var prefab))
            return prefab;

        prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"PrefabManager: не найден префаб по пути {path}");
            return null;
        }

        _cache[path] = prefab;
        return prefab;
    }

    /// <summary>
    /// Создаёт экземпляр загруженного префаба.
    /// </summary>
    public static GameObject Instantiate(string path, Transform parent = null)
    {
        var prefab = Load(path);
        if (prefab == null) return null;

        return Object.Instantiate(prefab, parent);
    }

    /// <summary>
    /// Загружает префаб из Resources по пути Prefabs/UI/
    /// </summary>
    public static GameObject LoadUI(string path)
    {
        return Load($"Prefabs/UI/{path}");
    }

    /// <summary>
    /// Очистить весь кэш (например при смене сцены).
    /// </summary>
    public static void ClearCache()
    {
        _cache.Clear();
    }
}


// // Кэш для папок (чтобы не перезагружать LoadAll)
// private static readonly Dictionary<string, GameObject[]> _folderCache = new();
//     _folderCache.Clear();
//  /// <summary>
//     /// Загружает все префабы из папки.
//     /// </summary>
//     public static GameObject[] LoadAll(string folder)
//     {
//         if (_folderCache.TryGetValue(folder, out var cached))
//             return cached;

//         var loaded = Resources.LoadAll<GameObject>(folder);
//         if (loaded == null || loaded.Length == 0)
//         {
//             Debug.LogWarning($"PrefabManager: в папке {folder} нет префабов");
//         }

//         _folderCache[folder] = loaded;
//         return loaded;
//     }

// /// <summary>
// /// Создаёт случайный префаб из папки.
// /// </summary>
// public static GameObject InstantiateRandom(string folder, Transform parent = null)
// {
//     var prefabs = LoadAll(folder);
//     if (prefabs.Length == 0) return null;

//     var prefab = prefabs[Random.Range(0, prefabs.Length)];
//     return Object.Instantiate(prefab, parent);
// }