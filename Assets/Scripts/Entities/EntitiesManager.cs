using System;
using System.Collections.Generic;
using System.IO;
using Entities.Locations;
using Entities.Streaming;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Контракт центральной подсистемы управления сущностями (EntitiesManager).
    /// Отвечает за:
    /// 1. Плоский реестр всех спящих сущностей сессии (уровень L0).
    /// 2. Оперативную память активных сущностей (уровень L1).
    /// 3. Пространственный стриминг (распаковка L0 -> L1 в радиусе внимания фокуса).
    /// 4. Индексацию по локациям и спавнерам для быстрого поиска и каскадных операций.
    /// </summary>
    public interface IEntities
    {
        /// <summary>
        /// Идентификатор текущей активной локации сессии (см. LocationIds).
        /// Определяет, сущности какой зоны загружены в активный CullingGroup/Streamer.
        /// </summary>
        string CurrentLocationId { get; }

        /// <summary>
        /// Полный плоский реестр всех существующих в сессии сущностей на уровне L0 (до 10 000 шт.).
        /// Легковесные структуры координат и шаблонов, сериализуемые в общий сейв сессии.
        /// </summary>
        IReadOnlyDictionary<string, EntityL0> EntitiesL0 { get; }

        /// <summary>
        /// Только активные размороженные сущности в оперативной памяти L1 (~100-200 шт.).
        /// Содержат компоненты, тиковые циклы и локальную шину событий.
        /// </summary>
        IReadOnlyDictionary<string, EntityL1> ActiveEntitiesL1 { get; }

        /// <summary>
        /// Индекс сущностей по спавнерам: SpawnerId -> Набор EntityId, порожденных данным спавнером.
        /// Используется для каскадного удаления (например, уничтожить всех миньонов при гибели босса)
        /// и контроля лимитов спавна (не спавнить больше N объектов).
        /// </summary>
        IReadOnlyDictionary<string, HashSet<string>> SpawnedEntities { get; }

        /// <summary>
        /// Регистрация спящей сущности L0 в реестре мира и индексах локации/спавнера.
        /// Если сущность принадлежит CurrentLocationId — она регистрируется в стримере.
        /// </summary>
        void RegisterL0(EntityL0 entity);

        /// <summary>
        /// Получение сущности по ID. Если сущность уже активна в L1 — возвращает её сразу.
        /// Если сущность спит в L0 — производит разморозку в оперативную память L1 (Load -> Init -> Start).
        /// </summary>
        EntityL1 GetEntity(string entityId);

        /// <summary>
        /// Полное удаление сущности из игры: деактивация, удаление из L0, стримера, индексов и удаление JSON-файла с диска.
        /// </summary>
        void RemoveEntity(string entityId);

        /// <summary>
        /// Разморозка сущности: L0 -> L1.
        /// Загружает состояние из JSON, связывает компоненты и запускает логику.
        /// </summary>
        EntityL1 Activate(string entityId);

        /// <summary>
        /// Заморозка сущности: L1 -> L0.
        /// Останавливает логику, сохраняет состояние в JSON на SSD и выгружает из RAM.
        /// </summary>
        EntityL1 Deactivate(string entityId);

        /// <summary>
        /// Получить все EntityId сущностей, находящихся в указанной локации.
        /// </summary>
        IReadOnlyCollection<string> GetEntitiesByLocation(string locationId);

        /// <summary>
        /// Получить все EntityId сущностей, порожденных данным спавнером.
        /// </summary>
        IReadOnlyCollection<string> GetEntitiesBySpawner(string spawnerId);

        /// <summary>
        /// Каскадное удаление всех сущностей, порожденных данным спавнером (снаряды, суммоны, ящики).
        /// </summary>
        void RemoveEntitiesBySpawner(string spawnerId);

        /// <summary>
        /// Смена активной локации в рамках сессии: замораживает старые сущности, перемещает фокусную сущность,
        /// перезаряжает стример сущностями новой локации и выполняет пространственную синхронизацию.
        /// </summary>
        void ChangeLocation(string newLocationId, Vector3 focusSpawnPoint, string focusEntityId = null);

        /// <summary>
        /// Сериализация реестра L0 и текущей локации в сохранение сессии.
        /// </summary>
        EntitiesData Save();

        /// <summary>
        /// Восстановление реестра сессии из сохранения.
        /// </summary>
        void Load(EntitiesData data);
    }

    /// <summary>
    /// Главный менеджер сущностей игры.
    /// Реализует разделение на спящий реестр (L0), активный рантайм (L1),
    /// индексацию по локациям/спавнерам и пространственный стриминг.
    /// </summary>
    public class EntitiesManager : IEntities, IDisposable
    {
        // 1. Плоский реестр всех сущностей сессии на уровне L0 (легковесные структуры в RAM, до 10 000)
        private readonly Dictionary<string, EntityL0> _entitiesL0 = new(10000);

        // 2. Только активные размороженные сущности в оперативной памяти L1 (~100-200 шт.)
        private readonly Dictionary<string, EntityL1> _activeEntitiesL1 = new(256);

        // 3. Индекс по локациям: LocationId -> Набор EntityId
        private readonly Dictionary<string, HashSet<string>> _locationMap = new();

        // 4. Индекс по спавнерам: SpawnerId -> Набор EntityId (для каскадной очистки и лимитов)
        private readonly Dictionary<string, HashSet<string>> _spawnerMap = new();

        /// <summary>Идентификатор текущей активной локации мира.</summary>
        public string CurrentLocationId { get; private set; } = LocationIds.Default;

        public IReadOnlyDictionary<string, EntityL0> EntitiesL0 => _entitiesL0;
        public IReadOnlyDictionary<string, EntityL1> ActiveEntitiesL1 => _activeEntitiesL1;
        public IReadOnlyDictionary<string, HashSet<string>> SpawnedEntities => _spawnerMap;

        /// <summary>Подсистема пространственного стриминга сущностей (CullingGroup или SpatialGrid).</summary>
        public IEntitiesStreamer Streamer { get; private set; }

        public EntitiesManager(StreamerType streamerType = StreamerType.CullingGroup)
        {
            SetStreamer(streamerType);
        }

        /// <summary>
        /// Переключение реализации пространственного стримера (CullingGroup или SpatialGrid).
        /// Автоматически регистрирует в стримере только сферы текущей активной локации.
        /// </summary>
        public void SetStreamer(StreamerType type)
        {
            Streamer?.Dispose();

            Streamer = type == StreamerType.CullingGroup
                ? new CullingGroupStreamer(this)
                : new SpatialGridStreamer(this);

            if (_locationMap.TryGetValue(CurrentLocationId, out var entityIds))
            {
                foreach (var id in entityIds)
                {
                    if (_entitiesL0.TryGetValue(id, out var l0))
                    {
                        Streamer.RegisterL0(l0);
                        if (IsActive(id))
                        {
                            Streamer.MuteL0(id);
                        }
                    }
                }
            }
        }

        /// <summary>Проверка, активна ли сущность в оперативной памяти (L1).</summary>
        public bool IsActive(string entityId) => !string.IsNullOrEmpty(entityId) && _activeEntitiesL1.ContainsKey(entityId);

        /// <summary>Попытка быстро получить спящий дескриптор L0 без активации сущности.</summary>
        public bool TryGetL0(string entityId, out EntityL0 l0)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                l0 = default;
                return false;
            }
            return _entitiesL0.TryGetValue(entityId, out l0);
        }

        /// <summary>Получить все EntityId сущностей указанной локации.</summary>
        public IReadOnlyCollection<string> GetEntitiesByLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId)) return Array.Empty<string>();
            return _locationMap.TryGetValue(locationId, out var set) ? set : (IReadOnlyCollection<string>)Array.Empty<string>();
        }

        /// <summary>Получить все EntityId сущностей, порожденных указанным спавнером.</summary>
        public IReadOnlyCollection<string> GetEntitiesBySpawner(string spawnerId)
        {
            if (string.IsNullOrEmpty(spawnerId)) return Array.Empty<string>();
            return _spawnerMap.TryGetValue(spawnerId, out var set) ? set : (IReadOnlyCollection<string>)Array.Empty<string>();
        }

        /// <summary>
        /// Регистрация спящей сущности L0 в реестре сессии и индексах.
        /// Если сущность относится к текущей локации — регистрирует её сферу в стримере.
        /// </summary>
        public void RegisterL0(EntityL0 l0)
        {
            if (string.IsNullOrEmpty(l0.EntityId)) return;

            if (string.IsNullOrEmpty(l0.LocationId))
            {
                l0.LocationId = LocationIds.Default;
            }

            // Обновляем индексы, если сущность уже была зарегистрирована с другими параметрами
            if (_entitiesL0.TryGetValue(l0.EntityId, out var oldL0))
            {
                if (oldL0.LocationId != l0.LocationId)
                {
                    RemoveFromLocationIndex(oldL0.LocationId, l0.EntityId);
                    if (oldL0.LocationId == CurrentLocationId)
                    {
                        Streamer.UnregisterL0(l0.EntityId);
                    }
                }
                if (oldL0.SpawnerId != l0.SpawnerId)
                {
                    RemoveFromSpawnerIndex(oldL0.SpawnerId, l0.EntityId);
                }
            }

            _entitiesL0[l0.EntityId] = l0;
            AddToLocationIndex(l0.LocationId, l0.EntityId);

            if (!string.IsNullOrEmpty(l0.SpawnerId))
            {
                AddToSpawnerIndex(l0.SpawnerId, l0.EntityId);
            }

            // В CullingGroup регистрируются сферы ТОЛЬКО текущей активной локации!
            if (l0.LocationId == CurrentLocationId)
            {
                Streamer.RegisterL0(l0);
                if (IsActive(l0.EntityId))
                {
                    Streamer.MuteL0(l0.EntityId);
                }
            }
        }

        /// <summary>
        /// Добавить готовую активную сущность в систему.
        /// </summary>
        public void AddEntity(EntityL1 l1)
        {
            if (l1 == null || string.IsNullOrEmpty(l1.EntityId)) return;

            RegisterL0(l1.L0);
            _activeEntitiesL1[l1.EntityId] = l1;
        }

        /// <summary>
        /// Получение сущности по ID. Если спит в L0 — производит разморозку в память L1.
        /// </summary>
        public EntityL1 GetEntity(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return null;

            if (_activeEntitiesL1.TryGetValue(entityId, out var activeEntity))
            {
                return activeEntity;
            }

            if (_entitiesL0.ContainsKey(entityId))
            {
                return Activate(entityId);
            }

            // Новая сущность, создаваемая впервые
            var newL0 = new EntityL0(entityId, locationId: CurrentLocationId);
            RegisterL0(newL0);
            return Activate(entityId);
        }

        /// <summary>
        /// Распаковка сущности (L0 -> L1):
        /// 1. Создание инстанса EntityL1 в памяти.
        /// 2. Загрузка компонентов из JSON (Фаза 1: Create & Load).
        /// 3. Setup (для новых сущностей).
        /// 4. Инициализация и линковка компонентов (Фаза 2: Init).
        /// 5. Запуск логики и подписка на события (Фаза 3: Start).
        /// 6. Глушение сферы в стримере (радиус 0, координаты вдаль).
        /// </summary>
        public EntityL1 Activate(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return null;

            if (_activeEntitiesL1.TryGetValue(entityId, out var existing))
            {
                return existing;
            }

            if (!_entitiesL0.TryGetValue(entityId, out var l0))
            {
                l0 = new EntityL0(entityId, locationId: CurrentLocationId);
                RegisterL0(l0);
            }

            var entityL1 = new EntityL1(l0);

            // Обработка специальных скриптовых сущностей (для NewGame.cs)
            if (entityId == "MainMenuInitScript" && entityL1.GetComponent<MainMenuInitScriptComponent>() == null)
            {
                entityL1.AddComponent(new MainMenuInitScriptComponent());
            }

            // Фаза 1: Загрузка личных данных компонентов из JSON
            entityL1.LoadComponents();

            if (!entityL1.IsConfigured)
            {
                entityL1.Setup();
            }

            // Фаза 2: Инициализация и безопасная линковка компонентов
            entityL1.InitComponents();

            // Фаза 3: Запуск работы компонентов
            entityL1.Start();

            _activeEntitiesL1[entityId] = entityL1;

            // Глушим сферу в стримере, так как сущность уже активна в памяти
            if (l0.LocationId == CurrentLocationId)
            {
                Streamer.MuteL0(entityId);
            }

            return entityL1;
        }

        /// <summary>
        /// Заморозка сущности (L1 -> L0):
        /// 1. Остановка логики компонентов (Фаза 4: Stop).
        /// 2. Сохранение состояния компонентов в JSON на SSD (Фаза 4: Save).
        /// 3. Обновление координат и локации в дескрипторе L0.
        /// 4. Пробуждение сферы в стримере (UnmuteL0).
        /// 5. Освобождение ресурсов RAM (Dispose).
        /// </summary>
        public EntityL1 Deactivate(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return null;
            if (!_activeEntitiesL1.TryGetValue(entityId, out var entityL1)) return null;

            entityL1.Stop();
            entityL1.SaveComponents();

            if (_entitiesL0.TryGetValue(entityId, out var l0))
            {
                string oldLocation = l0.LocationId;
                l0.LocalPosition = entityL1.Body.Position;
                l0.LocationId = entityL1.Body.LocationId;
                l0.IsConfigured = entityL1.IsConfigured;
                _entitiesL0[entityId] = l0;

                if (oldLocation != l0.LocationId)
                {
                    RemoveFromLocationIndex(oldLocation, entityId);
                    AddToLocationIndex(l0.LocationId, entityId);
                }

                if (l0.LocationId == CurrentLocationId)
                {
                    Streamer.UnmuteL0(entityId, l0.LocalPosition, l0.Radius);
                }
            }

            entityL1.Dispose();
            _activeEntitiesL1.Remove(entityId);

            return entityL1;
        }

        /// <summary>
        /// Полное удаление сущности из сессии и удаление ее файла JSON с диска.
        /// </summary>
        public void RemoveEntity(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return;

            Deactivate(entityId);

            if (_entitiesL0.TryGetValue(entityId, out var l0))
            {
                RemoveFromLocationIndex(l0.LocationId, entityId);
                if (!string.IsNullOrEmpty(l0.SpawnerId))
                {
                    RemoveFromSpawnerIndex(l0.SpawnerId, entityId);
                }
            }

            _entitiesL0.Remove(entityId);
            Streamer.UnregisterL0(entityId);

            try
            {
                string sessionId = GlobalGame.Session?.SessionId;
                if (!string.IsNullOrEmpty(sessionId))
                {
                    string filePath = Path.Combine(DataPathManager.Entities(sessionId), entityId + ".json");
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[EntitiesManager] Не удалось удалить файл сущности {entityId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Каскадное удаление всех сущностей, порожденных данным спавнером.
        /// </summary>
        public void RemoveEntitiesBySpawner(string spawnerId)
        {
            if (string.IsNullOrEmpty(spawnerId)) return;

            if (_spawnerMap.TryGetValue(spawnerId, out var spawnedSet))
            {
                var toRemove = new List<string>(spawnedSet);
                foreach (var entityId in toRemove)
                {
                    RemoveEntity(entityId);
                }
                _spawnerMap.Remove(spawnerId);
            }
        }

        /// <summary>
        /// Смена активной локации в рамках сессии:
        /// 1. Замораживает все активные сущности старой зоны (кроме фокусной).
        /// 2. Переносит фокусную сущность в новую локацию по координатам spawnPoint.
        /// 3. Очищает стример и регистрирует сферы новой локации.
        /// 4. Принудительно выполняет пространственную синхронизацию вокруг точки появления.
        /// </summary>
        public void ChangeLocation(string newLocationId, Vector3 focusSpawnPoint, string focusEntityId = null)
        {
            if (string.IsNullOrEmpty(newLocationId))
            {
                Debug.LogWarning("[EntitiesManager] Попытка переключиться на пустой LocationId!");
                return;
            }

            Debug.Log($"[EntitiesManager] Смена локации: {CurrentLocationId} -> {newLocationId}. Точка появления фокуса: {focusSpawnPoint}");

            var activeIds = new List<string>(_activeEntitiesL1.Keys);
            foreach (var activeId in activeIds)
            {
                if (!string.IsNullOrEmpty(focusEntityId) && activeId == focusEntityId)
                {
                    continue;
                }
                Deactivate(activeId);
            }

            if (!string.IsNullOrEmpty(focusEntityId) && _activeEntitiesL1.TryGetValue(focusEntityId, out var focusL1))
            {
                string oldLoc = focusL1.Body.LocationId;
                focusL1.Body.LocationId = newLocationId;
                focusL1.Body.Position = focusSpawnPoint;

                RemoveFromLocationIndex(oldLoc, focusEntityId);
                AddToLocationIndex(newLocationId, focusEntityId);

                if (_entitiesL0.TryGetValue(focusEntityId, out var focusL0))
                {
                    focusL0.LocationId = newLocationId;
                    focusL0.LocalPosition = focusSpawnPoint;
                    _entitiesL0[focusEntityId] = focusL0;
                }
            }

            CurrentLocationId = newLocationId;
            Streamer.Clear();

            if (_locationMap.TryGetValue(newLocationId, out var newEntities))
            {
                foreach (var entityId in newEntities)
                {
                    if (_entitiesL0.TryGetValue(entityId, out var l0))
                    {
                        Streamer.RegisterL0(l0);
                        if (IsActive(entityId))
                        {
                            Streamer.MuteL0(entityId);
                        }
                    }
                }
            }

            Streamer.ForceSyncAround(focusSpawnPoint);
        }

        /// <summary>Обновление стримера текущим положением фокуса.</summary>
        public void UpdateStreamer(Vector3 focusPosition, float deltaTime)
        {
            Streamer?.Update(focusPosition, deltaTime);
        }

        /// <summary>Принудительная пространственная синхронизация вокруг фокуса (снятие барьера).</summary>
        public void ForceSyncAround(Vector3 focusPosition, float overrideRadius = -1f)
        {
            Streamer?.ForceSyncAround(focusPosition, overrideRadius);
        }

        /// <summary>Сериализация реестра L0 и текущей локации в сейв сессии.</summary>
        public EntitiesData Save()
        {
            foreach (var l1 in _activeEntitiesL1.Values)
            {
                l1.SaveComponents();

                if (_entitiesL0.TryGetValue(l1.EntityId, out var l0))
                {
                    l0.LocalPosition = l1.Body.Position;
                    l0.LocationId = l1.Body.LocationId;
                    l0.IsConfigured = l1.IsConfigured;
                    _entitiesL0[l1.EntityId] = l0;
                }
            }

            var data = new EntitiesData
            {
                CurrentLocationId = CurrentLocationId
            };

            foreach (var l0 in _entitiesL0.Values)
            {
                data.EntitiesL0.Add(l0);
            }

            return data;
        }

        /// <summary>Восстановление реестра сессии из сохранения.</summary>
        public void Load(EntitiesData data)
        {
            _activeEntitiesL1.Clear();
            _entitiesL0.Clear();
            _locationMap.Clear();
            _spawnerMap.Clear();
            Streamer.Clear();

            if (data == null) return;

            CurrentLocationId = !string.IsNullOrEmpty(data.CurrentLocationId) ? data.CurrentLocationId : LocationIds.Default;

            if (data.EntitiesL0 != null && data.EntitiesL0.Count > 0)
            {
                foreach (var l0 in data.EntitiesL0)
                {
                    RegisterL0(l0);
                }
            }
        }

        private void AddToLocationIndex(string locationId, string entityId)
        {
            if (string.IsNullOrEmpty(locationId)) locationId = LocationIds.Default;
            if (!_locationMap.TryGetValue(locationId, out var set))
            {
                set = new HashSet<string>();
                _locationMap[locationId] = set;
            }
            set.Add(entityId);
        }

        private void RemoveFromLocationIndex(string locationId, string entityId)
        {
            if (string.IsNullOrEmpty(locationId)) return;
            if (_locationMap.TryGetValue(locationId, out var set))
            {
                set.Remove(entityId);
                if (set.Count == 0)
                {
                    _locationMap.Remove(locationId);
                }
            }
        }

        private void AddToSpawnerIndex(string spawnerId, string entityId)
        {
            if (string.IsNullOrEmpty(spawnerId)) return;
            if (!_spawnerMap.TryGetValue(spawnerId, out var set))
            {
                set = new HashSet<string>();
                _spawnerMap[spawnerId] = set;
            }
            set.Add(entityId);
        }

        private void RemoveFromSpawnerIndex(string spawnerId, string entityId)
        {
            if (string.IsNullOrEmpty(spawnerId)) return;
            if (_spawnerMap.TryGetValue(spawnerId, out var set))
            {
                set.Remove(entityId);
                if (set.Count == 0)
                {
                    _spawnerMap.Remove(spawnerId);
                }
            }
        }

        // --- Такты жизненного цикла активных L1 сущностей ---
        public void FixedUpdate()
        {
            foreach (var entity in _activeEntitiesL1.Values)
            {
                entity.FixedUpdate();
            }
        }

        public void Update()
        {
            foreach (var entity in _activeEntitiesL1.Values)
            {
                entity.Update();
            }
        }

        public void LateUpdate()
        {
            foreach (var entity in _activeEntitiesL1.Values)
            {
                entity.LateUpdate();
            }
        }

        public void PauseUpdate()
        {
            foreach (var entity in _activeEntitiesL1.Values)
            {
                entity.PauseUpdate();
            }
        }

        public void Dispose()
        {
            var activeKeys = new List<string>(_activeEntitiesL1.Keys);
            foreach (var key in activeKeys)
            {
                Deactivate(key);
            }

            _activeEntitiesL1.Clear();
            _entitiesL0.Clear();
            _locationMap.Clear();
            _spawnerMap.Clear();
            Streamer?.Dispose();
        }
    }

    /// <summary>
    /// Контейнер сериализации сущностей в общий файл сохранения сессии.
    /// Сохраняет только легкий реестр спящих сущностей L0 и текущую локацию.
    /// </summary>
    [Serializable]
    public class EntitiesData
    {
        public string CurrentLocationId = LocationIds.Default;
        public List<EntityL0> EntitiesL0 = new();
    }
}
