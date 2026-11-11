using System;

namespace Entities
{
    [Serializable]
    public class EntityInfoComponentData
    {
        public string Name = "";
        public string EntityType = "";
        public string SpawnTime = "";
    }

    public class EntityInfoComponent : EntityComponentBase
    {
        public string Name;
        public string EntityType;
        public DateTime SpawnTime;

        public override void Start()
        {
            if (SpawnTime == default)
                SpawnTime = DateTime.Now;
        }

        public override object Save()
        {
            EntityInfoComponentData data = new()
            {
                Name = Name,
                EntityType = EntityType,
                SpawnTime = SaveData.Date(SpawnTime)
            };

            return data;
        }

        public override void Load(object data)
        {
            EntityInfoComponentData EntityInfoComponentData = (EntityInfoComponentData)data;
            Name = EntityInfoComponentData.Name;
            EntityType = EntityInfoComponentData.EntityType;
            SpawnTime = SaveData.Date(EntityInfoComponentData.SpawnTime);
        }
    }
}