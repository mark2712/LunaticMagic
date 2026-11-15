using System;

namespace Entities
{
    /*
        АРХИТЕКТУРНОЕ ВИДЕНИЕ: IEntity (Уровень существования L0 -> L1)
        -------------------------------------------------------------
        Сущность на этом уровне — это ЧИСТЫЙ КОНТЕЙНЕР ДАННЫХ и идентификации.
        
        Она НЕ знает:
        - Есть ли у неё GameObject на сцене Unity
        - Есть ли у неё коллайдер или физика Rigidbody (L2 - L3)
        - Загружена ли 3D-модель / VRM (L4)
        - Находится ли она сейчас под управлением игрока (Фокус)
        
        Её задача:
        1. Хранить уникальный EntityId и метаданные для быстрого поиска в реестре.
        2. Предоставлять доступ к размороженным данным (Runtime / Components / Data).
        3. Сериализоваться в JSON файл на диске сессии при заморозке (L1 -> L0).
    */

    public interface IEntity
    {
        string EntityId { get; }
        string SpawnerId { get; }
        bool IsConfigured { get; set; }
        
        // Ссылка на размороженные C# данные/компоненты сущности в памяти (L1).
        // Если сущность спит на диске (L0), это поле равно null.
        IEntityRuntime EntityRuntime { get; set; }

        // Первичная инициализация структуры данных сущности (при самом первом создании)
        void Setup();
    }

    public interface IEntityUnique
    {
        string EntityId { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueEntityAttribute : Attribute
    {
    }

    // Легковесные метаданные реестра сущностей (хранятся в общем сейве сессии)
    [Serializable]
    public class EntityData
    {
        public string EntityId;
        public string SpawnerId;
        public bool IsConfigured;
    }

    // Заглушка для компиляции и безопасного обращения к несуществующим сущностям
    public class EntityStub : IEntity
    {
        public string EntityId { get; set; } = "Stub";
        public string SpawnerId { get; set; }
        public bool IsConfigured { get; set; }
        public IEntityRuntime EntityRuntime { get; set; } = new EntityRuntimeStub();
        public void Setup() { }
    }
}
