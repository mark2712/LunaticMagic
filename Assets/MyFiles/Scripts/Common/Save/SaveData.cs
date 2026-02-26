// public enum SaveDataType
// {
//     JSON,
//     Bin
// }

using System;
using System.Globalization;

public static class SaveData
{
    public static T Load<T>(string folderPath, string fileName) where T : class
    {
        return SaveDataJSON.Load<T>(folderPath, fileName);
    }

    public static void Save<T>(T data, string folderPath, string fileName)
    {
        SaveDataJSON.Save(data, folderPath, fileName);
    }


    // В JSON будет строго "yyyy-MM-dd HH:mm"
    // и это будет локальная дата/время компьютера пользователя на момент сохранения
    private const string Format = "yyyy-MM-dd HH:mm:ss";

    public static string Date(DateTime dateTime)
    {
        // Важно: НЕ ToUniversalTime()
        return dateTime.ToString(Format, CultureInfo.InvariantCulture);
    }

    public static DateTime Date(string dateTime)
    {
        if (string.IsNullOrWhiteSpace(dateTime))
            return DateTime.MinValue;

        return DateTime.ParseExact(dateTime, Format, CultureInfo.InvariantCulture);
    }
}
