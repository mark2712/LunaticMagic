using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


public enum CleanupLevel { Soft, Hard }

public interface ICleanable
{
    // Очистить только то, что явно не нужно (RefCount = 0 или старое)
    void CleanupSoft();

    // Очистить вообще всё, что можно выгрузить
    void CleanupHard();
}

public class ClearManager
{
    private List<ICleanable> _services = new();

    public void Register(ICleanable service) => _services.Add(service);

    public void RequestCleanup(CleanupLevel level)
    {
        foreach (var service in _services)
        {
            if (level == CleanupLevel.Soft) service.CleanupSoft();
            else service.CleanupHard();
        }

        if (level == CleanupLevel.Hard)
        {
            // В Unity это тяжелые операции
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}


public class ClearManagerMemory
{
    // Настройки для ПК (можно вынести в конфиг)
    private const float SOFT_CLEANUP_THRESHOLD = 0.7f; // 70% занято
    private const float HARD_CLEANUP_THRESHOLD = 0.9f; // 90% занято

    public void LateUpdate()
    {
        // Проверяем раз в несколько секунд, а не каждый кадр (для оптимизации)
        if (Time.frameCount % 300 != 0) return; 

        float memoryUsage = GetMemoryUsagePercentage();

        if (memoryUsage > HARD_CLEANUP_THRESHOLD)
        {
            // ExecuteCleanup(CleanupLevel.Hard);
        }
        else if (memoryUsage > SOFT_CLEANUP_THRESHOLD)
        {
            // ExecuteCleanup(CleanupLevel.Soft);
        }
    }

    private float GetMemoryUsagePercentage()
    {
        // Для Unity под ПК:
        long totalMemory = SystemInfo.systemMemorySize; // Общая RAM ПК в МБ
        long allocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
        return (float)allocatedMemory / totalMemory;
    }
}