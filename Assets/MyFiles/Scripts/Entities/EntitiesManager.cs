using System.Collections.Generic;
using System.Collections.Specialized;

namespace Entities
{
    public class EntitiesManager
    {
        public Dictionary<string, Entity> Entities = new();

        public Entity GetEntity(string id)
        {
            if (Entities.ContainsKey(id))
            {
                return Entities[id];
            }
            return null;
        }
    }
}