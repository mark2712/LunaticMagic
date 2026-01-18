using System.IO;
using UnityEngine;

public static class DataPathManager
{
    public static string Base => EnsureDirectory(Path.Combine(Application.persistentDataPath, "AllData")); // Основной путь к папке AllData

    public static string ModelsVRM => EnsureDirectory(Path.Combine(Base, "ModelsVRM"));
    public static string Localization => EnsureDirectory(Path.Combine(Base, "Localization"));

    // Подкаталог для сохранений
    public static string GlobalSaves => EnsureDirectory(Path.Combine(Base, "Saves"));
    public static string Profiles => EnsureDirectory(Path.Combine(GlobalSaves, "Profiles"));
    public static string GameProfileSaves(string profileId)
    {
        return EnsureDirectory(Path.Combine(Profiles, profileId, "Saves"));
    }
    public static string GameProfileSave(string profileId, string SaveId)
    {
        return EnsureDirectory(Path.Combine(GameProfileSaves(profileId), SaveId));
    }

    public static string Sessions => EnsureDirectory(Path.Combine(Base, "Sessions"));

    /// <summary>
    /// Гарантирует, что директория существует. Если нет — создаёт её.
    /// </summary>
    public static string EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Глубокая копия всей папки
    /// </summary>
    public static void CopyDirectory(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(sourceDir))
            return;

        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.Replace(sourceDir, targetDir));
        }

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            File.Copy(
                file,
                file.Replace(sourceDir, targetDir),
                overwrite: true
            );
        }
    }
}


//     public static string Base
//     {
//         get
//         {
// #if UNITY_EDITOR
//             return EnsureDirectory(Path.Combine(Application.dataPath, "AllData"));
// #else
//             return EnsureDirectory(Path.Combine(Application.persistentDataPath, "AllData"));
// #endif
//         }
//     }
