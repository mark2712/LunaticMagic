using UnityEngine;

namespace Entities
{
    public interface IEntityRuntime
    {
        GameObject EntityGameObject { get; }
    }

    public class EntityRuntime : IEntityRuntime
    {
        public GameObject EntityGameObject { get; private set; }

        public void Load()
        {

        }

        public void Save()
        {

        }
    }
}