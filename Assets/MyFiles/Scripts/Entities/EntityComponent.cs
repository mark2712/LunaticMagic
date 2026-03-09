using System;
using System.Collections.Generic;

namespace Entities
{
    public interface IEntityComponent : IDisposable
    {
        // void Init(); // Срабатывавет каждый раз при добавлении компонента. Данные компонента загружены, теперь их можно менять.
        void Start(); // создание GO, включение Update, AI...
        void Stop(); // уничтожение GO
        // void Dispose(); // Срабатывавет каждый раз при уничтожении компонента. Данные компонента сохранены, теперь их нельзя менять.
        string Save();
        void Load(string data);
    }

    [Serializable]
    public class EntityComponentsData
    {
        public List<ComponentSaveData> Components = new();
    }

    public class EntityComponentBase : IEntityComponent
    {
        // public virtual void Init() { }
        public virtual void Start() { }
        public virtual void Stop() { }
        public virtual void Dispose() { }
        public virtual void Load(string data) { }
        public virtual string Save() { return null; }
    }


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




    public class EntityComponentPhysicsBody : EntityComponentBase, IUpdateComponent, IFixedUpdateComponent
    {
        public void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}

/*
EntityNPC

EntityScript - выполняет скрипт
EntitySpawner - спавнит (создаёт) другие неуникальные сущности в некотором радиусе
EntityInventory - сущность имеет инвентарь
EntityStats - сущность имеет уровень, здоровье, сопротивление и тд, может получать урон
EntityPhysicsBody - сущность имеет физическое тело 
*/

