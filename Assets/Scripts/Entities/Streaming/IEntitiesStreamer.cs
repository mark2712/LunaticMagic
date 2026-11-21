using System;
using UnityEngine;

namespace Entities.Streaming
{
    public enum StreamerType
    {
        CullingGroup, // Нативный C++ движковый стриминг Unity (SIMD)
        SpatialGrid   // Управляемый чистый C# стриминг на 2D хэш-сетке (запасной)
    }

    // Общий контракт системы пространственного стриминга сущностей (распаковщика L0 -> L1)
    public interface IEntitiesStreamer : IDisposable
    {
        StreamerType Type { get; }
        float DefaultWakeRadius { get; set; }
        event Action<string> OnEntityUnpacked;

        void RegisterL0(EntityL0 entity);
        void UnregisterL0(string entityId);

        // "Глушение" сферы при переходе в L1 (радиус 0, координаты далеко)
        void MuteL0(string entityId);

        // "Пробуждение" сферы при возврате в L0 (восстановление честных координат)
        void UnmuteL0(string entityId, Vector3 position, float radius);

        void UpdateL0Position(string entityId, Vector3 newPosition);

        void Update(Vector3 focusPosition, float deltaTime);
        void ForceSyncAround(Vector3 focusPosition, float overrideRadius = -1f);
        void Clear();
    }
}
