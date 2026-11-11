using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UniVRM10;

public class VRMCharacterPrefabCreator : MonoBehaviour
{
    // Путь к папке с моделями
    private string modelsPath => Path.Combine(DataPathManager.ModelsVRM, "Source");

    // Структура для хранения модели и её координат
    [System.Serializable]
    public class VRMModelSpawnData
    {
        public string modelName; // Имя модели без расширения
        public Vector3 spawnPosition; // Позиция для спавна
    }

    // Список моделей для спавна
    public List<VRMModelSpawnData> modelsToSpawn = new List<VRMModelSpawnData>();

    // Список найденных моделей
    private List<string> availableModels = new List<string>();

    void Start()
    {
        // VRMCharactersLoad();
    }

    public void VRMCharactersLoad()
    {
        VRMCharacterPrefabCreator creator = GetComponent<VRMCharacterPrefabCreator>();

        // Загружаем доступные модели
        creator.LoadAvailableModels();

        // Добавляем модели для спавна
        // creator.modelsToSpawn.Add(new VRMModelSpawnData
        // {
        //     modelName = "qe14",
        //     spawnPosition = new Vector3(0, 0, 0)
        // });

        // creator.modelsToSpawn.Add(new VRMModelSpawnData
        // {
        //     modelName = "qe15",
        //     spawnPosition = new Vector3(1, 0, 0)
        // });

        // Спавним модели
        _ = creator.SpawnModelsAsync();
    }


    /// <summary>
    /// Получить список доступных моделей в папке.
    /// </summary>
    public void LoadAvailableModels()
    {
        if (!Directory.Exists(modelsPath))
        {
            Debug.LogError($"Папка {modelsPath} не найдена!");
            return;
        }

        // Ищем все файлы с расширением .vrm
        string[] vrmFiles = Directory.GetFiles(modelsPath, "*.vrm");

        availableModels.Clear();
        foreach (string file in vrmFiles)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            availableModels.Add(fileNameWithoutExtension);
        }

        Debug.Log($"Найдено моделей: {availableModels.Count}");
    }

    /// <summary>
    /// Спавн моделей на сцене.
    /// </summary>
    public async Task SpawnModelsAsync()
    {
        foreach (VRMModelSpawnData spawnData in modelsToSpawn)
        {
            // Проверяем, есть ли модель в списке доступных
            if (!availableModels.Contains(spawnData.modelName))
            {
                Debug.LogWarning($"Модель {spawnData.modelName} не найдена в папке!");
                continue;
            }

            string modelPath = Path.Combine(modelsPath, spawnData.modelName + ".vrm");

            GameObject modelPrefab = await LoadVRMModelAsync(modelPath);
            if (modelPrefab == null)
            {
                Debug.LogError($"Не удалось загрузить модель {spawnData.modelName}");
                continue;
            }

            modelPrefab.name = spawnData.modelName;
            modelPrefab.transform.position = spawnData.spawnPosition;
        }
    }

    private async Task<GameObject> LoadVRMModelAsync(string path)
    {
        Debug.Log($"Начинаем загрузку VRM-файла: {path}");

        // Загружаем модель
        var instance = await Vrm10.LoadPathAsync(
            path,
            controlRigGenerationOption: ControlRigGenerationOption.Generate,
            // awaitCaller: new ImmediateCaller(),
            showMeshes: true
        );

        // Проверяем, что модель успешно загружена
        if (instance != null)
        {
            Debug.Log("VRM-файл успешно загружен.");

            // Получаем загруженный GameObject
            GameObject model = instance.gameObject;
            return model;
        }

        return null;
    }
}





// creator.modelsToSpawn.Add(new VRMModelSpawnData
// {
//     modelName = "qe2 2",
//     spawnPosition = new Vector3(2, 0, 0)
// });

// creator.modelsToSpawn.Add(new VRMModelSpawnData
// {
//     modelName = "qe4 1",
//     spawnPosition = new Vector3(3, 0, 0)
// });

// creator.modelsToSpawn.Add(new VRMModelSpawnData
// {
//     modelName = "qe5 3",
//     spawnPosition = new Vector3(4, 0, 0)
// });

// creator.modelsToSpawn.Add(new VRMModelSpawnData
// {
//     modelName = "qe13",
//     spawnPosition = new Vector3(5, 0, 0)
// });