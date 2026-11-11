using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Entities
{
    public interface IEntity
    {
        public string EntityId { get; }
        public string SpawnerId { get; }
        public bool IsConfigured { get; set; }
        public IEntityRuntime EntityRuntime { get; set; }
        void Setup();
    }


    public class Entity : IEntity
    {
        public string EntityId { get; private set; }
        public bool IsActive { get; private set; }
        public string SpawnerId { get; private set; }
        public bool IsConfigured { get; set; }
        public IEntityRuntime EntityRuntime { get; set; }

        public Entity(EntityData entityData)
        {
            EntityId = entityData.EntityId;
            SpawnerId = entityData.SpawnerId;
            IsConfigured = entityData.IsConfigured;
        }

        public static string GenerateEntityId() => $"{GeneratorId.GenerateId("Entity")}_{DateTime.Now:yyyyMMdd_HHmmss}";

        public virtual void Setup()
        {
            Build();
            PostBuild();
        }

        protected virtual void Build()
        {
            EntityRuntime.AddComponent(new EntityInfoComponent());
        }
        protected virtual void PostBuild(){}
    }

    [Serializable]
    public class EntityData
    {
        public string EntityId = Entity.GenerateEntityId();
        public string SpawnerId;
        public bool IsConfigured = false;
    }
}
