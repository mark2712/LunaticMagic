// public enum SaveDataType
// {
//     JSON,
//     Bin
// }

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
}
