using System;

namespace ResourceSystem
{
    public interface IClearManager
    {
        /// <summary>
        /// Дефолтный порог времени неиспользования для Soft-очистки (30 секунд).
        /// </summary>
        TimeSpan DefaultSoftThreshold { get; set; }

        void Register(ICleanable service);

        void Unregister(ICleanable service);

        /// <summary>
        /// Запросить очистку ресурсов.
        /// </summary>
        /// <param name="level">Уровень очистки</param>
        /// <param name="unusedThreshold">Опциональный порог времени для Soft-очистки</param>
        void RequestCleanup(CleanupLevel level, TimeSpan? unusedThreshold = null);
    }
}
