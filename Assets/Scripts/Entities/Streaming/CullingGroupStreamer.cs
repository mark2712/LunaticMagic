using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Streaming
{
    // Нативный C++ движковый стриминг Unity на базе CullingGroup (SIMD).
    // Реализует паттерн "Muted Sphere" (заглушение сфер при переходе в L1)
    // и "Free-List Pool" (переиспользование слотов массива при полном удалении сущностей).
    public class CullingGroupStreamer : IEntitiesStreamer
    {
        public StreamerType Type => StreamerType.CullingGroup;

        // Координаты заглушения: сфера уносится бесконечно далеко, исключая срабатывание событий
        public static readonly Vector3 MutedPosition = new(0, -999999f, 0);

        private float _defaultWakeRadius;
        public float DefaultWakeRadius
        {
            get => _defaultWakeRadius;
            set
            {
                _defaultWakeRadius = value;
                _cullingGroup?.SetBoundingDistances(new float[] { _defaultWakeRadius });
            }
        }

        public event Action<string> OnEntityUnpacked;

        private readonly EntitiesManager _entitiesManager;
        private CullingGroup _cullingGroup;

        // Непрерывный плоский массив сфер для нативного C++ ядра Unity
        private BoundingSphere[] _spheres = new BoundingSphere[1024];

        // Двусторонний индекс Index <-> EntityId за O(1)
        private string[] _indexToId = new string[1024];
        private readonly Dictionary<string, int> _idToIndex = new(1024);

        // Пул освободившихся слотов (Free-List) для повторного использования уничтоженных сущностей
        private readonly Stack<int> _freeSlots = new(128);

        private int _count = 0;

        public CullingGroupStreamer(EntitiesManager entitiesManager, float defaultWakeRadius = 200f)
        {
            _entitiesManager = entitiesManager ?? throw new ArgumentNullException(nameof(entitiesManager));
            _defaultWakeRadius = defaultWakeRadius;

            InitCullingGroup();
        }

        private void InitCullingGroup()
        {
            _cullingGroup = new CullingGroup();
            _cullingGroup.SetBoundingSpheres(_spheres);
            _cullingGroup.SetBoundingSphereCount(_count);
            _cullingGroup.SetBoundingDistances(new float[] { _defaultWakeRadius });
            _cullingGroup.onStateChanged = OnCullingGroupStateChanged;
        }

        // Регистрация спящей сущности L0 в массиве сфер CullingGroup
        public void RegisterL0(EntityL0 entity)
        {
            if (string.IsNullOrEmpty(entity.EntityId)) return;

            // Если уже зарегистрирована — просто обновляем сферу
            if (_idToIndex.TryGetValue(entity.EntityId, out int existingIdx))
            {
                float existingRadius = entity.Radius > 0f ? entity.Radius : _defaultWakeRadius;
                _spheres[existingIdx] = new BoundingSphere(entity.LocalPosition, existingRadius);
                return;
            }

            int idx;
            // 1. Проверяем, есть ли освободившийся слот в пуле (от ранее удаленного моба/лута)
            if (_freeSlots.Count > 0)
            {
                idx = _freeSlots.Pop();
            }
            else
            {
                // 2. Иначе берем новый слот в конце массива (с расширением при необходимости)
                if (_count >= _spheres.Length)
                {
                    int newCapacity = _spheres.Length * 2;
                    Array.Resize(ref _spheres, newCapacity);
                    Array.Resize(ref _indexToId, newCapacity);
                    _cullingGroup.SetBoundingSpheres(_spheres);
                }

                idx = _count++;
                _cullingGroup.SetBoundingSphereCount(_count);
            }

            float radius = entity.Radius > 0f ? entity.Radius : _defaultWakeRadius;
            _spheres[idx] = new BoundingSphere(entity.LocalPosition, radius);
            _indexToId[idx] = entity.EntityId;
            _idToIndex[entity.EntityId] = idx;
        }

        // Полное удаление сущности (уничтожение моба/лута навсегда):
        // Сфера глушится, а её слот отправляется в пул _freeSlots для новых сущностей
        public void UnregisterL0(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            if (_idToIndex.TryGetValue(entityId, out int idx))
            {
                // Глушим сферу за 20 км с радиусом 0
                _spheres[idx] = new BoundingSphere(MutedPosition, 0f);
                _indexToId[idx] = null;
                _idToIndex.Remove(entityId);

                // Возвращаем слот в пул для будущих спавнов
                _freeSlots.Push(idx);
            }
        }

        // Заглушение сферы при переходе в L1:
        // Слот сохраняется за этой сущностью, но сфера уносится вдаль, чтобы C++ не спамил событиями
        public void MuteL0(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            if (_idToIndex.TryGetValue(entityId, out int idx))
            {
                _spheres[idx] = new BoundingSphere(MutedPosition, 0f);
            }
        }

        // Пробуждение сферы при возврате в L0 (засыпание сущности обратно на диск):
        // Возвращаются честные координаты и радиус интереса
        public void UnmuteL0(string entityId, Vector3 position, float radius)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            if (_idToIndex.TryGetValue(entityId, out int idx))
            {
                float r = radius > 0f ? radius : _defaultWakeRadius;
                _spheres[idx] = new BoundingSphere(position, r);
            }
        }

        // Быстрое обновление координат спящей сферы за O(1) (1 запись в память)
        public void UpdateL0Position(string entityId, Vector3 newPosition)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            if (_idToIndex.TryGetValue(entityId, out int idx))
            {
                _spheres[idx].position = newPosition;
            }
        }

        // Задание точки отсчета дистанции (позиция точки фокуса / игрока)
        public void Update(Vector3 focusPosition, float deltaTime)
        {
            _cullingGroup?.SetDistanceReferencePoint(focusPosition);
        }

        // Событие от C++ ядра Unity при пересечении дистанционной полосы
        private void OnCullingGroupStateChanged(CullingGroupEvent evt)
        {
            // evt.currentDistance == 0 означает вход в зону DefaultWakeRadius
            if (evt.currentDistance == 0)
            {
                int idx = evt.index;
                if (idx < 0 || idx >= _count) return;

                string entityId = _indexToId[idx];
                if (string.IsNullOrEmpty(entityId)) return;

                // Если сущность еще спит в L0 — глушим сферу и распаковываем сущность в L1!
                if (!_entitiesManager.IsActive(entityId))
                {
                    // Мгновенно глушим сферу прямо перед активацией, чтобы исключить повторные триггеры
                    _spheres[idx] = new BoundingSphere(MutedPosition, 0f);

                    _entitiesManager.Activate(entityId);
                    OnEntityUnpacked?.Invoke(entityId);
                }
            }
        }

        // Принудительное мгновенное сканирование (для спавна, телепорта или барьера загрузки)
        public void ForceSyncAround(Vector3 focusPosition, float overrideRadius = -1f)
        {
            _cullingGroup?.SetDistanceReferencePoint(focusPosition);

            float radius = overrideRadius > 0f ? overrideRadius : _defaultWakeRadius;
            float sqrRadius = radius * radius;

            for (int i = 0; i < _count; i++)
            {
                // Пропускаем свободные или заглушенные слоты
                string entityId = _indexToId[i];
                if (string.IsNullOrEmpty(entityId)) continue;
                if (_entitiesManager.IsActive(entityId)) continue;

                if ((_spheres[i].position - focusPosition).sqrMagnitude <= sqrRadius)
                {
                    _spheres[i] = new BoundingSphere(MutedPosition, 0f);
                    _entitiesManager.Activate(entityId);
                    OnEntityUnpacked?.Invoke(entityId);
                }
            }
        }

        // Полная очистка и пересоздание CullingGroup (вызывается при загрузке сейва / перезапуске сессии)
        public void Clear()
        {
            _count = 0;
            _freeSlots.Clear();
            _idToIndex.Clear();
            Array.Clear(_indexToId, 0, _indexToId.Length);

            _cullingGroup?.Dispose();
            _cullingGroup = null;

            InitCullingGroup();
        }

        public void Dispose()
        {
            _count = 0;
            _freeSlots.Clear();
            _idToIndex.Clear();
            Array.Clear(_indexToId, 0, _indexToId.Length);

            _cullingGroup?.Dispose();
            _cullingGroup = null;
        }
    }
}
