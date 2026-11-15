using System;

namespace Entities
{
    /*
        АРХИТЕКТУРНОЕ ВИДЕНИЕ: IEntityComponent
        --------------------------------------
        Компоненты на уровне системы сущностей отвечают за хранение и модификацию 
        данных конкретного аспекта сущности (Stats, Inventory, Attributes, AI State).
        
        Каждый компонент умеет:
        - Сериализовать свои данные в JSON (Save)
        - Восстанавливать свои данные из JSON (Load)
        
        Внешние системы (Focus, Streaming L2-L4, Presentation) читают эти компоненты
        и на их основе создают в мире Unity физику, камеры или визуальные модели.
    */

    public interface IEntityComponent : IDisposable
    {
        void Start();
        void Stop();
        object Save();
        void Load(object data);
    }

    public class EntityComponentBase : IEntityComponent
    {
        public virtual void Start() { }
        public virtual void Stop() { }
        public virtual void Dispose() { }
        public virtual void Load(object data) { }
        public virtual object Save() => null;
    }

    // Тактовые интерфейсы для компонентов, требующих обновления в фоновом режиме (A-Life)
    public interface IFixedUpdateComponent
    {
        void FixedUpdate();
    }

    public interface IPauseUpdateComponent
    {
        void PauseUpdate();
    }

    public interface IUpdateComponent
    {
        void Update();
    }

    public interface ILateUpdateComponent
    {
        void LateUpdate();
    }

    // Заглушка тестового скрипта инициализации главного меню (для NewGame.cs)
    public class MainMenuInitScriptComponent : EntityComponentBase
    {
        public void Execute() { }
    }
}
