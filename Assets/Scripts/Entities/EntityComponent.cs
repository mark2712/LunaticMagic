using System;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Контракт логического компонента сущности (уровень L1).
    /// Компоненты хранят состояние и логику конкретного аспекта (HP, инвентарь, статы, ИИ).
    /// 
    /// Жизненный цикл строго подчинен правилу 4-х фаз:
    /// 1. Load(jsonData)   - Фаза 1: десериализация данных из JSON в полной изоляции (соседей не ищем!).
    /// 2. Init(owner)      - Фаза 2: линковка с EntityL1 и безопасный поиск соседей через GetComponent.
    /// 3. Start()          - Фаза 3: подписка на EntityEventBus, запуск тикеров и таймеров.
    /// 4. Stop() / Save()  - Фаза 4: отписка от событий, сериализация состояния в JSON и Dispose.
    /// </summary>
    public interface IEntityComponent : IDisposable
    {
        /// <summary>Ссылка на сущность-владельца (L1).</summary>
        EntityL1 Entity { get; }

        /// <summary>Завершена ли Фаза 2 (Init).</summary>
        bool IsInitialized { get; }

        /// <summary>Активна ли Фаза 3 (Start).</summary>
        bool IsStarted { get; }

        /// <summary>Фаза 1: Загрузка персональных данных из строки JSON (до связывания с соседями).</summary>
        void Load(string jsonData);

        /// <summary>Фаза 2: Линковка к сущности. Все компоненты уже созданы, безопасно кэшируем соседей.</summary>
        void Init(EntityL1 owner);

        /// <summary>Фаза 3: Старт работы, подписка на события шины.</summary>
        void Start();

        /// <summary>Фаза 4: Остановка работы, отписка от событий.</summary>
        void Stop();

        /// <summary>Фаза 4: Сериализация состояния в строку JSON для сохранения на SSD.</summary>
        string Save();
    }

    /// <summary>
    /// Базовая реализация логического компонента L1.
    /// Предоставляет реализацию по умолчанию для 4-х фаз жизненного цикла.
    /// </summary>
    public abstract class EntityComponentBase : IEntityComponent
    {
        public EntityL1 Entity { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsStarted { get; private set; }

        public virtual void Load(string jsonData) { }
        public virtual string Save() => null;

        public virtual void Init(EntityL1 owner)
        {
            Entity = owner;
            IsInitialized = true;
        }

        public virtual void Start()
        {
            IsStarted = true;
        }

        public virtual void Stop()
        {
            IsStarted = false;
        }

        public virtual void Dispose()
        {
            if (IsStarted)
            {
                Stop();
            }
            Entity = null;
            IsInitialized = false;
        }
    }

    /// <summary>
    /// Обобщенный базовый класс для компонентов с типизированными данными состояния.
    /// Автоматически сериализует и десериализует объект состояния TData через JsonUtility.
    /// </summary>
    public abstract class EntityComponent<TData> : EntityComponentBase where TData : class, new()
    {
        /// <summary>Данные состояния компонента, сохраняемые на диск.</summary>
        protected TData Data { get; private set; } = new();

        public override void Load(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    Data = JsonUtility.FromJson<TData>(jsonData) ?? new TData();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[{GetType().Name}] Ошибка десериализации данных: {ex.Message}");
                    Data = new TData();
                }
            }
            else
            {
                Data = new TData();
            }
        }

        public override string Save()
        {
            return Data != null ? JsonUtility.ToJson(Data) : null;
        }
    }

    /// <summary>Компонент, требующий регулярного тика в цикле FixedUpdate.</summary>
    public interface IFixedUpdateComponent
    {
        void FixedUpdate();
    }

    /// <summary>Компонент, требующий обновления при активной паузе сессии.</summary>
    public interface IPauseUpdateComponent
    {
        void PauseUpdate();
    }

    /// <summary>Компонент, требующий покадрового тика в цикле Update.</summary>
    public interface IUpdateComponent
    {
        void Update();
    }

    /// <summary>Компонент, требующий тика в конце кадра LateUpdate.</summary>
    public interface ILateUpdateComponent
    {
        void LateUpdate();
    }

    /// <summary>Служебный компонент инициализации меню (вызывается из NewGame.cs).</summary>
    public class MainMenuInitScriptComponent : EntityComponentBase
    {
        public void Execute()
        {
            Debug.Log("[MainMenuInitScriptComponent] Инициализация меню выполнена.");
        }
    }
}
