using System;
using System.Collections.Generic;

namespace Entities
{
    public interface IEntities
    {
        IReadOnlyDictionary<string, IEntity> Entities { get; }
        IReadOnlyDictionary<string, IEntity> ActiveEntities { get; }
        IReadOnlyDictionary<string, string> SpawnedEntities { get; }
        IEntity GetEntity(IEntity entity);
        IEntity GetEntity(string id);
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

        public void AddEntity(IEntity entity)
        {
            _entities.Add(entity.EntityId, entity);
            if (entity.SpawnerId != null)
            {
                _spawnedEntities.Add(entity.SpawnerId, entity.EntityId);
            }
        }

        public IEntity GetEntity(IEntity entity) => GetEntity(entity.EntityId);
        public IEntity GetEntity(string id)
        {
            IEntity entity;
            if (_entities.ContainsKey(id))
            {
                entity = _entities[id];
                if (!entity.IsActive)
                {
                    entity.Activate();
                }
                return entity;
            }
            return null;
        }

        public void RemoveEntity(string id)
        {
            IEntity entity = _entities[id];
            entity.Deactivate();
            _entities.Remove(id);
            _activeEntities.Remove(id);
            _spawnedEntities.Remove(id);
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
            foreach (var entity in _activeEntities)
            {
                entity.Value.EntityRuntime.Dispose();
            }
        }

        public void AddActiveEntity(string entityId) => _activeEntities[entityId] = _entities[entityId];
        public void RemoveActiveEntity(string entityId) => _activeEntities.Remove(entityId);

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

    // public interface IEntityBuilder
    // {
    //     IEntity Entity { get; }
    //     IEntityRuntime Runtime { get; }
    //     IEntity Activate();
    //     IEntity Deactivate();
    // }

    // public class EntityBuilder : IEntityBuilder
    // {
    //     public IEntity Entity { get; private set; }
    //     private IEntities Entities => GlobalGame.Session.EntitiesManager;
    //     public IEntityRuntime Runtime => Entity.EntityRuntime;

    //     public EntityBuilder(string id)
    //     {
    //         Entity = Entities.Entities[id];
    //     }

    //     public IEntity Activate()
    //     {
    //         Entities.Activate(Entity);
    //         return Entity;
    //     }

    //     public IEntity Deactivate()
    //     {
    //         Entities.Deactivate(Entity);
    //         return Entity;
    //     }
    // }
}


