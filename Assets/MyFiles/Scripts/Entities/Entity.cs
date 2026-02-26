using UnityEngine;

namespace Entities
{
    public interface IEntity
    {
        public string EntityId { get; }
        public string LocationId { get; }
        public EntityRuntime EntityRuntime { get; } // body, stats, position, vectors...
        void Load();
        void Save();
    }


    public class Entity : IEntity
    {
        public string EntityId { get; private set; }
        public string LocationId { get; private set; }
        public EntityRuntime EntityRuntime { get; private set; }

        public void Load()
        {

        }

        public void Save()
        {

        }
    }
}