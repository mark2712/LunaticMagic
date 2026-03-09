using System;
using System.Collections.Generic;

namespace Entities
{
    public class EntityBuilder
    {
        private readonly List<IEntityComponent> _components = new();

        private string _entityId;
        private string _spawnerId;

        public EntityBuilder WithId(string id)
        {
            _entityId = id;
            return this;
        }

        public EntityBuilder WithSpawner(string spawnerId)
        {
            _spawnerId = spawnerId;
            return this;
        }

        public EntityBuilder AddComponent<T>() where T : IEntityComponent, new()
        {
            _components.Add(new T());
            return this;
        }

        public EntityBuilder AddComponent(IEntityComponent component)
        {
            _components.Add(component);
            return this;
        }

        public IEntity Build()
        {
            EntityData data = new()
            {
                EntityId = _entityId ?? Entity.GenerateEntityId(),
                SpawnerId = _spawnerId
            };

            Entity entity = new(data);

            // сразу активируем
            entity.Activate();

            foreach (var component in _components)
            {
                entity.EntityRuntime.AddComponent(component);
            }

            return entity;
        }
    }
}