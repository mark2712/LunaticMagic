using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ResourceSystem
{
    /// <summary>
    /// Менеджер централизованной очистки ресурсов.
    /// Управляет регистрацией ICleanable сервисов и распределением запросов на очистку.
    /// </summary>
    public sealed class ClearManager : IClearManager
    {
        private readonly List<ICleanable> _services = new();
        private readonly ClearManagerMemory _memoryMonitor;

        /// <summary>
        /// Монитор памяти для ПК (Windows/Linux) с бюджетом 4-6 ГБ.
        /// </summary>
        public ClearManagerMemory MemoryMonitor => _memoryMonitor;

        /// <summary>
        /// Дефолтный порог времени неиспользования для Soft-очистки (300 секунд).
        /// </summary>
        public TimeSpan DefaultSoftThreshold { get; set; } = TimeSpan.FromSeconds(300);

        public ClearManager()
        {
            _memoryMonitor = new ClearManagerMemory(this);
            Application.lowMemory += _memoryMonitor.OnLowMemory;
            Observable.Interval(TimeSpan.FromSeconds(20))
                .Subscribe(_ => _memoryMonitor.CheckMemoryAndCleanup());
        }

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
}
