using System;

namespace Entities.Streaming
{
    // Класс-адаптер для обратной совместимости.
    // Наследуется от SpatialGridStreamer.
    public class EntitiesStreamer : SpatialGridStreamer
    {
        public EntitiesStreamer(EntitiesManager entitiesManager, float cellSize = 100f, float defaultWakeRadius = 200f)
            : base(entitiesManager, cellSize, defaultWakeRadius) { }
    }
}
