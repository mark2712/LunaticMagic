using System;
using System.Linq;
using System.Reflection;

namespace Entities
{
    public class EntitiesLoader
    {
        // Регистрирует всех сущностей по Id, без запуска Runtime
        // после запуска сесии и EntitiesManager.Load в реестре сущностей может не быть всех сущностей, особенно при старте новой игры
        public void RegisterMissingEntities(EntitiesManager entitiesManager)
        {
            var uniqueTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<UniqueEntityAttribute>() != null);

            foreach (var type in uniqueTypes)
            {
                var newEntity = (IEntity)Activator.CreateInstance(type); // Создает инстанс через пустой конструктор
                if (!entitiesManager.Entities.ContainsKey(newEntity.EntityId))
                {
                    entitiesManager.AddEntity(newEntity);
                }
            }
        }

        // создание Runtime (компонентов и данных). 
        // Сейчас сущности не общаются друг с другом. 
        // Так же тут происходит настройка уникальных сущностей
        public void InitEntitiesRuntime(EntitiesManager entitiesManager)
        {
            foreach (var entity in entitiesManager.Entities.Values)
            {
                if (!entity.IsConfigured)
                {
                    // Новая сущность: запускаем рантайм для первоначальной настройки
                    entitiesManager.Activate(entity.EntityId);

                    entity.Setup();

                    // Сохраняем начальные данные компонентов на диск
                    entity.EntityRuntime.SaveComponents();
                    entity.IsConfigured = true;

                    // Если сущность не должна быть активна по умолчанию, можно отключить её:
                    // entitiesManager.Deactivate(entity.EntityId);
                }
                else
                {
                    // Сущность уже настроена, загружаем её данные из файлов
                    entitiesManager.Activate(entity.EntityId);
                    entity.EntityRuntime.LoadComponents();
                }
            }
        }
    }
}