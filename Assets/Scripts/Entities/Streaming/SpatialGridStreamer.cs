using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Streaming
{
    // Запасной управляемый чистый C# стриминг на базе 2D пространственной хэш-сетки
    public class SpatialGridStreamer : IEntitiesStreamer
    {
        public StreamerType Type => StreamerType.SpatialGrid;

        public float DefaultWakeRadius { get; set; } = 200f;
        public float UpdateInterval { get; set; } = 0.25f;

        public event Action<string> OnEntityUnpacked;

        private readonly EntitiesManager _entitiesManager;
        private readonly SpatialHashGrid _grid;
        private readonly HashSet<string> _queryBuffer = new();

        private float _lastUpdateTime = -1f;

        public SpatialGridStreamer(EntitiesManager entitiesManager, float cellSize = 100f, float defaultWakeRadius = 200f)
        {
            _entitiesManager = entitiesManager ?? throw new ArgumentNullException(nameof(entitiesManager));
            _grid = new SpatialHashGrid(cellSize);
            DefaultWakeRadius = defaultWakeRadius;
        }

        public void RegisterL0(EntityL0 entity)
        {
            _grid.Insert(entity.EntityId, entity.LocalPosition);
        }

        public void UnregisterL0(string entityId)
        {
            _grid.Remove(entityId);
        }

        // Заглушение при переходе в L1: временно убираем из сетки поиска спящих
        public void MuteL0(string entityId)
        {
            _grid.Remove(entityId);
        }

        // Пробуждение при возврате в L0: возвращаем в сетку поиска спящих
        public void UnmuteL0(string entityId, Vector3 position, float radius)
        {
            _grid.Insert(entityId, position);
        }

        public void UpdateL0Position(string entityId, Vector3 newPosition)
        {
            _grid.UpdatePosition(entityId, newPosition);
        }

        public void Update(Vector3 focusPosition, float deltaTime)
        {
            _lastUpdateTime += deltaTime;
            if (_lastUpdateTime < UpdateInterval) return;

            _lastUpdateTime = 0f;
            ScanAndUnpack(focusPosition, DefaultWakeRadius);
        }

        public void ForceSyncAround(Vector3 focusPosition, float overrideRadius = -1f)
        {
            float radius = overrideRadius > 0f ? overrideRadius : DefaultWakeRadius;
            ScanAndUnpack(focusPosition, radius);
        }

        private void ScanAndUnpack(Vector3 focusPosition, float wakeRadius)
        {
            _grid.QueryRadius(focusPosition, wakeRadius, _queryBuffer);

            foreach (var entityId in _queryBuffer)
            {
                if (_entitiesManager.IsActive(entityId)) continue;

                if (_entitiesManager.TryGetL0(entityId, out var l0))
                {
                    float effectiveRadius = l0.Radius > 0f ? l0.Radius : wakeRadius;
                    float sqrDist = (l0.LocalPosition - focusPosition).sqrMagnitude;

                    if (sqrDist <= effectiveRadius * effectiveRadius)
                    {
                        MuteL0(entityId);
                        _entitiesManager.Activate(entityId);
                        OnEntityUnpacked?.Invoke(entityId);
                    }
                }
            }
        }

        public void Clear()
        {
            _grid.Clear();
            _queryBuffer.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
