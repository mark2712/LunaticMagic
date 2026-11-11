using System;
using UnityEngine;

namespace MobsOld
{
    public class Mob : MonoBehaviour
    {
        public string MobName { get; private set; } // имя моба (бочка, игрок, гоблин)
        public string MobId { get; private set; } // уникальный id
        public Body Body { get; set; } // физическое тело (коллайдеры, контроллеры, модель)
        public Entities_1.BaseEntity Entity { get; set; } // статы, получение урона, сопротивления, баффы

        // инвентари
        // навыки

        public Mob()
        {
            MobId = Guid.NewGuid().ToString();
        }

        public void SpawnMob(string name, Vector3 position)
        {

        }
        public void IsSpawnMob() { }

        public void SpawnPhysicsBody() { }
        public void IsSpawnPhysicsBody() { }

        public void LoadMob(string str)
        {
            // Mob data = JsonUtility.FromJson<Mob>(str);
            // Body.Load(data.Body);
            // Entity.Load(data.Entity);
        }
        
    }
}