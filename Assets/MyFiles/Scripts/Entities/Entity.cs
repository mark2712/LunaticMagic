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
        public IEntityRuntime EntityRuntime { get; set; }
    }


    public class Entity : IEntity
    {
        public string EntityId { get; private set; }
        public bool IsActive { get; private set; }
        public string SpawnerId { get; private set; }
        public IEntityRuntime EntityRuntime { get; set; }

        public Entity(EntityData entityData)
        {
            EntityId = entityData.EntityId;
            SpawnerId = entityData.SpawnerId;
        }

        public static string GenerateEntityId() => $"{GeneratorId.GenerateId("Entity")}_{DateTime.Now:yyyyMMdd_HHmmss}";
    }

    [Serializable]
    public class EntityData
    {
        public string EntityId = Entity.GenerateEntityId();
        public string SpawnerId;
    }
}
