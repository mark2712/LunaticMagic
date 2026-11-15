using System;

namespace ResourceSystem
{
    public interface ICleanable
    {
        /// <summary>
        /// Выполнить очистку ресурсов в соответствии с уровнем и порогом простоя.
        /// </summary>
        /// <param name="level">Уровень очистки (Soft, Hard, Emergency)</param>
        /// <param name="unusedThreshold">Порог времени неиспользования (для уровня Soft)</param>
        void Cleanup(CleanupLevel level, TimeSpan unusedThreshold);
    }
}
