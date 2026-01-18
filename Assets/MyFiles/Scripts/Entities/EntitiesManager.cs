using System.Collections.Generic;
using System.Collections.Specialized;

namespace Entities
{
    public class EntitiesManager
    {
        public Dictionary<string, EntityContainer> Entities = new();

        public EntityContainer GetEntity(string id)
        {
            if (Entities.ContainsKey(id))
            {
                return Entities[id];
            }
            return null;
        }
    }
}