using System;
using System.Collections.Generic;

namespace Entities
{
    public class UniqueEntitiesList
    {
        // Список шаблонов уникальных сущностей
        private readonly List<EntityTemplateBase> _uniqueEntities = new()
        {
            new EntityNewGameInitScript(),
            new EntityMainMenuInitScript(),
            new EntityZhorik(),
            new EntityGivi()
        };

        // private readonly List<Type> _uniqueEntities1 = new()
        // {
        //     typeof(EntityNewGameInitScript),
        //     typeof(EntityMainMenuInitScript),
        //     typeof(EntityZhorik),
        //     typeof(EntityGivi)
        // };

        // Метод для сравнения и добавления (вызывается 1 раз в начале новой игры)
        public void RegisterMissingEntities(EntitiesManager entitiesManager)
        {
            foreach (var entity in _uniqueEntities)
            {
                if (entity is IEntityUnique uniqueEntity)
                {
                    if (!entitiesManager.Entities.ContainsKey(uniqueEntity.EntityId))
                    {
                        var newEntity = entity.Create();
                        entitiesManager.AddEntity(newEntity);
                    }
                }
            }
        }
    }
}
