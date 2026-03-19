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

        IEntity Activate(string entityId); // загружает данные из json, создает компоненты
        IEntity Deactivate(string entityId); // загружает данные в json, уничтожает компоненты
    }

    public class EntitiesManager : IEntities, IDisposable
    {
        /* 
            Список ВСЕХ существующих в игре сущностей. Их ограниченое количество в игре. 
            Есть 2 вида - уникальные (ограниченое количество) и неуникальные (уничтожаются по условиям).
        */
        private readonly Dictionary<string, IEntity> _entities = new(); // <id сущности, сущность>
        private readonly Dictionary<string, IEntity> _activeEntities = new(); // <id сущности, сущность>
        private readonly Dictionary<string, string> _spawnedEntities = new(); // <id спавнера, id сущности> - для быстрого поиска

        public IReadOnlyDictionary<string, IEntity> Entities => _entities;
        public IReadOnlyDictionary<string, IEntity> ActiveEntities => _activeEntities;
        public IReadOnlyDictionary<string, string> SpawnedEntities => _spawnedEntities;

        private bool IsActive(string entityId) => _activeEntities.ContainsKey(entityId);
        private string EntityPath(string entityId) => Path.Combine(DataPathManager.Entities(GlobalGame.Session.SessionId), entityId + ".json");


        public void AddEntity(IEntity entity)
        {
            // добавить просто id
            _entities.Add(entity.EntityId, (Entity)entity);
            if (entity.SpawnerId != null)
            {
                _spawnedEntities.Add(entity.SpawnerId, entity.EntityId);
            }
            // проверить стейт и в зависимости от него активировать сущность
        }

        public IEntity GetEntity(IEntity entity) => GetEntity(entity.EntityId);
        public IEntity GetEntity(string entityId)
        {
            if (_entities.ContainsKey(entityId))
            {
                Entity entity = (Entity)_entities[entityId];
                if (!entity.IsActive)
                {
                    Activate(entityId);
                }
                return entity;
            }
            return null;
        }

        public IEntity Activate(string entityId)
        {
            IEntity entity = _entities[entityId];
            if (IsActive(entityId)) return entity;
            entity.EntityRuntime = new EntityRuntime(entity);
            _activeEntities[entityId] = _entities[entityId];
            return entity;
        }

        public IEntity Deactivate(string entityId)
        {
            IEntity entity = _entities[entityId];
            if (!IsActive(entityId)) return entity;
            entity.EntityRuntime = null;
            _activeEntities.Remove(entityId);
            return entity;
        }

        public void RemoveEntity(string entityId)
        {
            IEntity entity = _entities[entityId];
            Deactivate(entityId);
            _entities.Remove(entityId);
            _activeEntities.Remove(entityId);
            if (entity.SpawnerId != null)
            {
                _spawnedEntities.Remove(entity.SpawnerId);
            }
            // удалить свой файл
            string filePath = EntityPath(entityId);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }


        public void Load(EntitiesData data)
        {
            foreach (var entityData in data.Entities)
            {
                IEntity entity = new Entity(entityData);
                AddEntity(entity);
            }
        }

        public EntitiesData Save()
        {
            // сохранить (обновить файлы) данные всех активных сущностей (у неактивных уже есть актуальная информация в файлах)
            foreach (var entity in _activeEntities)
            {
                entity.Value.EntityRuntime?.SaveComponents(entity.Value);
            }
            // просто сохранить (верунть) список id всех сущностейи их простые данные
            EntitiesData data = new();
            foreach (var entity in _entities)
            {
                EntityData entityData = new()
                {
                    EntityId = entity.Value.EntityId,
                    SpawnerId = entity.Value.SpawnerId
                };
                data.Entities.Add(entityData);
            }
            return data;
        }

        public void Dispose()
        {
            foreach (var entityKV in _activeEntities)
            {
                IEntity entity = entityKV.Value;
                string entityId = entity.EntityId;
                if (IsActive(entityId)) Deactivate(entityId);
            }
        }

        public void FixedUpdate() { foreach (var entity in _activeEntities.Values) entity.EntityRuntime.FixedUpdate(); }
        public void PauseUpdate() { foreach (var entity in _activeEntities.Values) entity.EntityRuntime.PauseUpdate(); }
        public void Update() { foreach (var entity in _activeEntities.Values) entity.EntityRuntime.Update(); }
        public void LateUpdate() { foreach (var entity in _activeEntities.Values) entity.EntityRuntime.LateUpdate(); }
    }


    [Serializable]
    public class EntitiesData
    {
        public List<EntityData> Entities = new();
    }
}


// public interface EntityFluentBuilder
// {
//     public string EntityId { get; }
//     EntitiesManager EntitiesManager { get; }
//     EntityFluentBuilder Get();
//     EntityFluentBuilder Activate();
//     EntityFluentBuilder Deactivate();
// }
