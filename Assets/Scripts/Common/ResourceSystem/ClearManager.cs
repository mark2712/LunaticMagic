using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace ResourceSystem
{
    /// <summary>
    /// Уровни очистки ресурсов:
    /// - Soft: мягкая очистка неиспользуемых ресурсов (RefCount == 0), простаивающих больше заданного порога времени (например, меню паузы, инвентарь).
    /// - Hard: полная выгрузка всех неиспользуемых ресурсов (RefCount == 0) независимо от времени (например, смена уровня, выгрузка сессии).
    /// - Emergency: экстренная выгрузка всех неиспользуемых ресурсов + Resources.UnloadUnusedAssets() + GC.Collect() (при нехватке памяти).
    /// </summary>
    public enum CleanupLevel
    {
        Soft,
        Hard,
        Emergency
    }

    public interface ICleanable
    {
        /// <summary>
        /// Выполнить очистку ресурсов в соответствии с уровнем и порогом простоя.
        /// </summary>
        /// <param name="level">Уровень очистки (Soft, Hard, Emergency)</param>
        /// <param name="unusedThreshold">Порог времени неиспользования (для уровня Soft)</param>
        void Cleanup(CleanupLevel level, TimeSpan unusedThreshold);
    }

    /// <summary>
    /// Менеджер централизованной очистки ресурсов.
    /// Управляет регистрацией ICleanable сервисов и распределением запросов на очистку.
    /// </summary>
    public sealed class ClearManager
    {
        private readonly List<ICleanable> _services = new();

        /// <summary>
        /// Дефолтный порог времени неиспользования для Soft-очистки (30 секунд).
        /// </summary>
        public TimeSpan DefaultSoftThreshold { get; set; } = TimeSpan.FromSeconds(30);

        public void Register(ICleanable service)
        {
            if (service != null && !_services.Contains(service))
            {
                _services.Add(service);
            }
        }

        public void Unregister(ICleanable service)
        {
            if (service != null)
            {
                _services.Remove(service);
            }
        }

        /// <summary>
        /// Запросить очистку ресурсов.
        /// </summary>
        /// <param name="level">Уровень очистки</param>
        /// <param name="unusedThreshold">Опциональный порог времени для Soft-очистки</param>
        public void RequestCleanup(CleanupLevel level, TimeSpan? unusedThreshold = null)
        {
            var threshold = unusedThreshold ?? DefaultSoftThreshold;

            for (int i = 0; i < _services.Count; i++)
            {
                _services[i].Cleanup(level, threshold);
            }

            if (level == CleanupLevel.Emergency)
            {
                // В Unity это тяжелые операции (выполняются только при экстренной нехватке памяти)
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }
    }

    /// <summary>
    /// Система мониторинга использования оперативной памяти.
    /// </summary>
    public class ClearManagerMemory
    {
        private readonly ClearManager _clearManager;

        public float SoftMemoryThreshold { get; set; } = 0.7f;      // 70% занято
        public float EmergencyMemoryThreshold { get; set; } = 0.9f; // 90% занято

        public ClearManagerMemory(ClearManager clearManager)
        {
            _clearManager = clearManager;
        }

        /// <summary>
        /// Проверить уровень загрузки памяти и, при необходимости, запросить соответствующую очистку.
        /// </summary>
        public void CheckMemoryAndCleanup()
        {
            float memoryUsage = GetMemoryUsagePercentage();

            if (memoryUsage >= EmergencyMemoryThreshold)
            {
                _clearManager?.RequestCleanup(CleanupLevel.Emergency);
            }
            else if (memoryUsage >= SoftMemoryThreshold)
            {
                _clearManager?.RequestCleanup(CleanupLevel.Soft);
            }
        }

        /// <summary>
        /// Возвращает долю используемой приложением памяти относительно общего объема RAM (0.0 .. 1.0).
        /// </summary>
        public float GetMemoryUsagePercentage()
        {
            long totalMemory = SystemInfo.systemMemorySize; // Общая системная RAM в МБ
            if (totalMemory <= 0) return 0f;

            long allocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
            return (float)allocatedMemory / totalMemory;
        }
    }
}