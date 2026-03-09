using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Entities
{
    public interface IEntity
    {
        public string EntityId { get; }
        public bool IsActive { get; } // сейчас загружен рантайм?
        public string SpawnerId { get; }
        public IEntityRuntime EntityRuntime { get; }
        IEntity Activate(); // загружает данные из json, уничтожает json, запускает EntityRuntime.Start()
        IEntity Deactivate(); // загружает данные в json, запускает EntityRuntime.Dispose()
        void Remove();
    }


    public class Entity : IEntity
    {
        public string EntityId { get; private set; }
        public bool IsActive { get; private set; }
        public string SpawnerId { get; private set; }
        public IEntityRuntime EntityRuntime { get; private set; }
        private EntitiesManager EntitiesManager => GlobalGame.Session.EntitiesManager;

        public Entity(EntityData entityData)
        {
            EntityId = entityData.EntityId;
            SpawnerId = entityData.SpawnerId;
        }

        public static string GenerateEntityId()
        {
            return $"{GeneratorId.GenerateId("Entity")}_{DateTime.Now:yyyyMMdd_HHmmss}";
        }

        public IEntity Activate()
        {
            if (IsActive) return this;
            EntityRuntime = new EntityRuntime();
            EntityRuntime.LoadComponents(this);
            IsActive = true;
            EntitiesManager.AddActiveEntity(EntityId);
            EntityRuntime.Start();
            return this;
        }

        public IEntity Deactivate()
        {
            if (!IsActive) return this;
            EntityRuntime.SaveComponents(this);
            EntitiesManager.RemoveActiveEntity(EntityId);
            EntityRuntime.Dispose();
            EntityRuntime = null;
            IsActive = false;
            return this;
        }

        public void Remove()
        {
            EntitiesManager.RemoveEntity(EntityId);
        }
    }

    [Serializable]
    public class EntityData
    {
        public string EntityId = Entity.GenerateEntityId();
        public string SpawnerId;
    }
}

