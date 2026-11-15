using System;
using System.Collections.Generic;
using System.IO;

namespace Entities
{
    public interface IEntities
    {
        IReadOnlyDictionary<string, IEntity> Entities { get; }
        IReadOnlyDictionary<string, IEntity> ActiveEntities { get; }
        IReadOnlyDictionary<string, string> SpawnedEntities { get; }

        EntitiesData Save();
        void Load(EntitiesData data);

        void AddEntity(IEntity entity);
        IEntity GetEntity(IEntity entity);
        IEntity GetEntity(string entityId);
        void RemoveEntity(string entityId);

        IEntity Activate(string entityId); // загружает данные из json, создает компоненты (разморозка L0 -> L1)
        IEntity Deactivate(string entityId); // сохраняет данные в json, уничтожает компоненты (заморозка L1 -> L0)
    }

    public class EntitiesManager : IEntities, IDisposable
    {
        /* 
            Список ВСЕХ существующих в игре сущностей. Их ограниченное количество в игре. 
            Есть 2 вида - уникальные (ограниченное количество) и неуникальные (уничтожаются по условиям).
            
            Зона ответственности системы сущностей (L0 <-> L1):
            1. Хранение плоского реестра существующих сущностей в сессии.
            2. Разморозка (L0 -> L1): загрузка детального состояния сущности из файла сессии на диске в оперативную память.
            3. Заморозка (L1 -> L0): сохранение состояния сущности в файл и выгрузка из памяти.
            4. Создание новых и удаление уничтоженных сущностей.
        */
        private readonly Dictionary<string, IEntity> _entities = new(); // <id сущности, сущность>
        private readonly Dictionary<string, IEntity> _activeEntities = new(); // <id сущности, активная сущность в памяти L1>
        private readonly Dictionary<string, string> _spawnedEntities = new(); // <id спавнера, id сущности> - для быстрого поиска

        public IReadOnlyDictionary<string, IEntity> Entities => _entities;
        public IReadOnlyDictionary<string, IEntity> ActiveEntities => _activeEntities;
        public IReadOnlyDictionary<string, string> SpawnedEntities => _spawnedEntities;

        private bool IsActive(string entityId) => _activeEntities.ContainsKey(entityId);
        private string EntityPath(string entityId) => Path.Combine(DataPathManager.Entities(GlobalGame.Session.SessionId), entityId + ".json");

        public void AddEntity(IEntity entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.EntityId)) return;

            // добавить сущность в общий реестр
            _entities[entity.EntityId] = entity;
            if (entity.SpawnerId != null)
            {
                _spawnedEntities[entity.SpawnerId] = entity.EntityId;
            }
            // проверить стейт и в зависимости от него активировать сущность
        }

        public IEntity GetEntity(IEntity entity) => entity != null ? GetEntity(entity.EntityId) : null;

        public IEntity GetEntity(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return null;

            if (_entities.TryGetValue(entityId, out var entity))
            {
                // Если сущность ещё спит (L0), размораживаем её в память (L1)
                if (!IsActive(entityId))
                {
                    Activate(entityId);
                }
                return entity;
            }

            // Заглушка на случай обращения к новой/незарегистрированной сущности
            var stub = new EntityStub { EntityId = entityId };
            AddEntity(stub);
            return stub;
        }

        public IEntity Activate(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return null;

            if (!_entities.TryGetValue(entityId, out var entity))
            {
                entity = new EntityStub { EntityId = entityId };
                AddEntity(entity);
            }

            if (IsActive(entityId)) return entity;

            // Разморозка: инициализация рантайма сущности (загрузка данных с диска)
            entity.EntityRuntime ??= new EntityRuntimeStub();
            entity.EntityRuntime.LoadComponents();

            _activeEntities[entityId] = entity;
            return entity;
        }

        public IEntity Deactivate(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return null;
            if (!_entities.TryGetValue(entityId, out var entity)) return null;
            if (!IsActive(entityId)) return entity;

            // Заморозка: сохранение данных на диск и освобождение ресурсов
            entity.EntityRuntime?.SaveComponents();
            entity.EntityRuntime?.Dispose();
            entity.EntityRuntime = null;

            _activeEntities.Remove(entityId);
            return entity;
        }

        public void RemoveEntity(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) return;
            if (!_entities.TryGetValue(entityId, out var entity)) return;

            Deactivate(entityId);
            _entities.Remove(entityId);
            _activeEntities.Remove(entityId);

            if (entity.SpawnerId != null)
            {
                _spawnedEntities.Remove(entity.SpawnerId);
            }

            // удалить файл данных сущности с диска
            try
            {
                string filePath = EntityPath(entityId);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки отсутствия сессии/файла в тестах или редакторе
            }
        }

        public void Load(EntitiesData data)
        {
            if (data?.Entities == null) return;

            foreach (var entityData in data.Entities)
            {
                IEntity entity = new EntityStub
                {
                    EntityId = entityData.EntityId,
                    SpawnerId = entityData.SpawnerId,
                    IsConfigured = entityData.IsConfigured
                };
                AddEntity(entity);
            }
        }

        public EntitiesData Save()
        {
            // Сохранить (обновить файлы) данные всех активных сущностей
            // (у неактивных L0 уже есть актуальная информация в файлах на диске)
            foreach (var entity in _activeEntities)
            {
                entity.Value.EntityRuntime?.SaveComponents();
            }

            // Сохранить (вернуть) список id всех сущностей и их базовые метаданные реестра
            EntitiesData data = new();
            foreach (var entity in _entities)
            {
                EntityData entityData = new()
                {
                    EntityId = entity.Value.EntityId,
                    SpawnerId = entity.Value.SpawnerId,
                    IsConfigured = entity.Value.IsConfigured
                };
                data.Entities.Add(entityData);
            }
            return data;
        }

        public void Dispose()
        {
            // Заморозить и выгрузить все активные сущности
            var activeIds = new List<string>(_activeEntities.Keys);
            foreach (var entityId in activeIds)
            {
                if (IsActive(entityId))
                {
                    Deactivate(entityId);
                }
            }
        }

        // Обновление рантаймов (сохранено для совместимости с циклом GameSession)
        public void FixedUpdate() 
        { 
            foreach (var entity in _activeEntities.Values) 
                entity.EntityRuntime?.FixedUpdate(); 
        }

        public void PauseUpdate() 
        { 
            foreach (var entity in _activeEntities.Values) 
                entity.EntityRuntime?.PauseUpdate(); 
        }

        public void Update() 
        { 
            foreach (var entity in _activeEntities.Values) 
                entity.EntityRuntime?.Update(); 
        }

        public void LateUpdate() 
        { 
            foreach (var entity in _activeEntities.Values) 
                entity.EntityRuntime?.LateUpdate(); 
        }
    }

    [Serializable]
    public class EntitiesData
    {
        public List<EntityData> Entities = new();
    }
}

// Fluent builder идеи на будущее:
// public interface EntityFluentBuilder
// {
//     public string EntityId { get; }
//     EntitiesManager EntitiesManager { get; }
//     EntityFluentBuilder Get();
//     EntityFluentBuilder Activate();
//     EntityFluentBuilder Deactivate();
// }
