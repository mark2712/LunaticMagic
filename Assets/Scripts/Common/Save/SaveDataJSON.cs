using UnityEngine;
using System.IO;
using System;


public static class SaveDataJSON
{
    private static readonly Logger Logger = new("SaveDataJSON");

    public static void Save<T>(T data, string folderPath, string fileName)
    {
        try
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(folderPath, fileName + ".json");

            string jsonData = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(fullPath, jsonData);
            Logger.Info($"Данные успешно сохранены в {fullPath}");
        }
        catch (Exception e)
        {
            Logger.Error($"Ошибка сохранения данных: {e.Message}");
        }
    }

    public static T Load<T>(string folderPath, string fileName) where T : class
    {
        string fullPath = Path.Combine(folderPath, fileName + ".json");

        if (!File.Exists(fullPath))
        {
            return null;
            // Logger.Warning($"Файл {fullPath} не найден. Возвращаю default(T).");
            // return new T();
        }

        try
        {
            string jsonData = File.ReadAllText(fullPath);
            T loadedData = JsonUtility.FromJson<T>(jsonData);
            // Logger.Info($"Данные успешно загружены из {fullPath}");
            return loadedData;
        }
        catch (Exception e)
        {
            Logger.Error($"Ошибка загрузки данных: {e.Message}");
            return null;
            // return new T();
        }
    }
}