using UnityEngine;
using UnityEngine.Profiling;

namespace ResourceSystem
{
    /// <summary>
    /// Система мониторинга выделенной памяти для ПК (Windows / Linux).
    /// Отслеживает реальный объем памяти, занятый игрой (в МБ), с учетом бюджета в 4-6 ГБ.
    /// </summary>
    public class ClearManagerMemory
    {
        private readonly ClearManager _clearManager;

        /// <summary>
        /// Порог мягкой очистки: 4 ГБ (4096 МБ). При превышении выгружаются давно неиспользуемые ресурсы.
        /// </summary>
        public long SoftMemoryThresholdMb { get; set; } = 4096;

        /// <summary>
        /// Порог экстренной очистки: 6 ГБ (6144 МБ). При превышении вызывается Emergency очистка.
        /// </summary>
        public long EmergencyMemoryThresholdMb { get; set; } = 6144;

        public ClearManagerMemory(ClearManager clearManager)
        {
            _clearManager = clearManager;
        }

        /// <summary>
        /// Обработчик критического события ОС Application.lowMemory.
        /// Срабатывает при системном дефиците памяти и сразу вызывает Emergency очистку без доп. проверок.
        /// </summary>
        public void OnLowMemory()
        {
            Debug.LogWarning("[ResourceSystem.ClearManager] Получен сигнал Application.lowMemory от ОС! Экстренная Emergency очистка ресурсов.");
            _clearManager?.RequestCleanup(CleanupLevel.Emergency);
        }

        /// <summary>
        /// Проверить объем выделенной игрой памяти (МБ) и запросить очистку при превышении бюджета.
        /// </summary>
        public void CheckMemoryAndCleanup()
        {
            long allocatedMb = GetAllocatedMemoryMb();

            if (allocatedMb >= EmergencyMemoryThresholdMb)
            {
                Debug.LogWarning($"[ResourceSystem.ClearManager] Память игры ({allocatedMb} МБ) превысила Emergency порог ({EmergencyMemoryThresholdMb} МБ). Экстренная очистка.");
                _clearManager?.RequestCleanup(CleanupLevel.Emergency);
            }
            else if (allocatedMb >= SoftMemoryThresholdMb)
            {
                _clearManager?.RequestCleanup(CleanupLevel.Soft);
            }
        }

        /// <summary>
        /// Возвращает объем памяти (в МБ), выделенный текущим процессом игры.
        /// </summary>
        public long GetAllocatedMemoryMb()
        {
            return Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
        }
    }
}
