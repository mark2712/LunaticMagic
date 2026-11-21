using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Streaming
{
    // Ключ ячейки двумерной пространственной сетки
    public readonly struct GridCellKey : IEquatable<GridCellKey>
    {
        public readonly int X;
        public readonly int Z;

        public GridCellKey(int x, int z)
        {
            X = x;
            Z = z;
        }

        public bool Equals(GridCellKey other) => X == other.X && Z == other.Z;

        public override bool Equals(object obj) => obj is GridCellKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Z);

        public override string ToString() => $"Cell({X}, {Z})";
    }

    // 2D Пространственная хэш-сетка для сверхбыстрого поиска спящих сущностей L0 без перебора 10 000 объектов
    public class SpatialHashGrid
    {
        public float CellSize { get; }

        private readonly Dictionary<GridCellKey, HashSet<string>> _cells = new();
        private readonly Dictionary<string, GridCellKey> _entityToCell = new();

        public SpatialHashGrid(float cellSize = 100f)
        {
            CellSize = cellSize > 0f ? cellSize : 100f;
        }

        public GridCellKey GetCellKey(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.x / CellSize);
            int z = Mathf.FloorToInt(position.z / CellSize);
            return new GridCellKey(x, z);
        }

        public void Insert(string entityId, Vector3 position)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            var key = GetCellKey(position);
            if (!_cells.TryGetValue(key, out var set))
            {
                set = new HashSet<string>();
                _cells[key] = set;
            }

            set.Add(entityId);
            _entityToCell[entityId] = key;
        }

        public void Remove(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            if (_entityToCell.TryGetValue(entityId, out var key))
            {
                if (_cells.TryGetValue(key, out var set))
                {
                    set.Remove(entityId);
                    if (set.Count == 0)
                    {
                        _cells.Remove(key);
                    }
                }
                _entityToCell.Remove(entityId);
            }
        }

        public void UpdatePosition(string entityId, Vector3 newPosition)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            var newKey = GetCellKey(newPosition);
            if (_entityToCell.TryGetValue(entityId, out var oldKey))
            {
                if (oldKey.Equals(newKey)) return; // Остался в той же ячейке

                if (_cells.TryGetValue(oldKey, out var oldSet))
                {
                    oldSet.Remove(entityId);
                    if (oldSet.Count == 0) _cells.Remove(oldKey);
                }
            }

            if (!_cells.TryGetValue(newKey, out var newSet))
            {
                newSet = new HashSet<string>();
                _cells[newKey] = newSet;
            }

            newSet.Add(entityId);
            _entityToCell[entityId] = newKey;
        }

        public void QueryRadius(Vector3 center, float radius, HashSet<string> resultSet)
        {
            if (resultSet == null) return;
            resultSet.Clear();

            int minX = Mathf.FloorToInt((center.x - radius) / CellSize);
            int maxX = Mathf.FloorToInt((center.x + radius) / CellSize);
            int minZ = Mathf.FloorToInt((center.z - radius) / CellSize);
            int maxZ = Mathf.FloorToInt((center.z + radius) / CellSize);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    var key = new GridCellKey(x, z);
                    if (_cells.TryGetValue(key, out var set))
                    {
                        foreach (var id in set)
                        {
                            resultSet.Add(id);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            _cells.Clear();
            _entityToCell.Clear();
        }
    }
}
