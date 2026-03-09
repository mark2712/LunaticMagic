using System;

namespace Entities
{
    [Serializable]
    public class EntityInfoComponentData
    {
        public string Name;
        public string Description;
        public string SpawnTime;
    }

    public class EntityInfoComponent : EntityComponentBase
    {
        public string Name;
        public string Description;
        public DateTime SpawnTime;

        public override void Start()
        {
            if (SpawnTime == default)
                SpawnTime = DateTime.Now;
        }

        public override string Save()
        {
            EntityInfoComponentData data = new()
            {
                Name = Name,
                Description = Description,
                SpawnTime = SaveData.Date(SpawnTime)
            };

            return SaveData.SaveStr(data);
        }

        public override void Load(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            var data = SaveData.LoadStr<EntityInfoComponentData>(json);

            Name = data.Name;
            Description = data.Description;
            SpawnTime = SaveData.Date(data.SpawnTime);
        }
    }
}