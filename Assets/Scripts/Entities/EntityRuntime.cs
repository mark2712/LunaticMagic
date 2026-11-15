using System;
using System.Collections.Generic;

namespace Entities
{
    /*
        АРХИТЕКТУРНОЕ ВИДЕНИЕ: IEntityRuntime (Размороженное состояние L1)
        -----------------------------------------------------------------
        Это объект чистых данных и чистого C# состояния в оперативной памяти.
        
        Здесь живут:
        - Характеристики (HP, мана, выносливость, уровень)
        - Инвентарь и экипировка
        - Флаги диалогов, отношения к фракциям, состояние AI (A-Life вне физики)
        - Данные для виртуальных камер и UI (которые активируются только если сущность получит Фокус)
        
        Чего здесь НЕТ:
        - Физических компонентов Unity (Collider, Rigidbody, NavMeshAgent)
        - Визуальных объектов (MeshRenderer, SkinnedMeshRenderer, Animator)
        - Обработки пользовательского ввода напрямую (ввод поступает через Фокус)
    */

    public interface IEntityRuntime : IDisposable
    {
        IEntity Entity { get; }
        bool IsStarted { get; }
        bool IsDisposed { get; }

        // Доступ к данным и логическим подсистемам размороженной сущности
        IEnumerable<IEntityComponent> GetAllComponents();
        T GetComponent<T>() where T : class, IEntityComponent;
        void AddComponent(IEntityComponent component);
        void RemoveComponent<T>() where T : IEntityComponent;

        // Загрузка и сохранение детальных данных сущности в/из JSON-файла сессии
        void LoadComponents();
        void SaveComponents();

        // Опциональные такты симуляции чистого C# (A-Life, таймеры баффов)
        void FixedUpdate();
        void PauseUpdate();
        void Update();
        void LateUpdate();
    }

    public class EntityRuntimeStub : IEntityRuntime
    {
        public IEntity Entity { get; }
        public bool IsStarted => false;
        public bool IsDisposed => false;

        public IEnumerable<IEntityComponent> GetAllComponents() => Array.Empty<IEntityComponent>();
        public T GetComponent<T>() where T : class, IEntityComponent => null;
        public void AddComponent(IEntityComponent component) { }
        public void RemoveComponent<T>() where T : IEntityComponent { }
        public void LoadComponents() { }
        public void SaveComponents() { }
        public void FixedUpdate() { }
        public void PauseUpdate() { }
        public void Update() { }
        public void LateUpdate() { }
        public void Dispose() { }
    }
}
