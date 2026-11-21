using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Контракт активной сущности в оперативной памяти (уровень L1).
    /// Предоставляет доступ к компонентам, пространственным координатам (Body) и локальным событиям (Events).
    /// </summary>
    public interface IEntity
    {
        /// <summary>Уникальный строковый идентификатор сущности.</summary>
        string EntityId { get; }

        /// <summary>Ссылка на шаблон / префаб сущности (если создана по шаблону).</summary>
        string TemplateId { get; }

        /// <summary>Идентификатор спавнера (null для уникальных сущностей мира).</summary>
        string SpawnerId { get; set; }

        /// <summary>Флаг первичной настройки компонентов сущности.</summary>
        bool IsConfigured { get; set; }

        /// <summary>Физический скафандр сущности (координаты, поворот, привязка к L2/L3).</summary>
        BodyComponent Body { get; }

        /// <summary>Локальная шина сигналов сущности для связи компонентов без жестких ссылок.</summary>
        EntityEventBus Events { get; }

        /// <summary>Служебный геттер для совместимости вызова entity.EntityRuntime.</summary>
        IEntity EntityRuntime => this;

        /// <summary>Флаг завершения Фазы 2 (Init) для всех компонентов.</summary>
        bool IsInitialized { get; }

        /// <summary>Флаг активности Фазы 3 (Start) — работают ли такты и события.</summary>
        bool IsStarted { get; }

        /// <summary>Флаг уничтожения/освобождения сущности.</summary>
        bool IsDisposed { get; }

        /// <summary>Получить коллекцию всех прикрепленных компонентов.</summary>
        IEnumerable<IEntityComponent> GetAllComponents();

        /// <summary>Получить компонент указанного типа (или null, если отсутствует).</summary>
        T GetComponent<T>() where T : class, IEntityComponent;

        /// <summary>Добавить компонент к сущности с автоматической интеграцией в жизненный цикл.</summary>
        void AddComponent(IEntityComponent component);

        /// <summary>Удалить компонент указанного типа с корректной остановкой (Stop) и Dispose.</summary>
        void RemoveComponent<T>() where T : class, IEntityComponent;

        /// <summary>Первичная инициализация компонентов по умолчанию (для новых сущностей).</summary>
        void Setup();

        /// <summary>Фаза 2: связывание всех компонентов и безопасный поиск зависимостей.</summary>
        void InitComponents();

        /// <summary>Фаза 3: запуск работы компонентов и подписка на события.</summary>
        void Start();

        /// <summary>Фаза 4: остановка работы компонентов и отписка от событий.</summary>
        void Stop();

        /// <summary>Фаза 1: загрузка состояния компонентов из персонального JSON-файла на SSD.</summary>
        void LoadComponents();

        /// <summary>Фаза 4: сохранение состояния компонентов в персональный JSON-файл на SSD.</summary>
        void SaveComponents();

        void FixedUpdate();
        void PauseUpdate();
        void Update();
        void LateUpdate();
    }

    /// <summary>Интерфейс для совместимости со скриптами вызовов EntityRuntime.</summary>
    public interface IEntityRuntime : IEntity { }

    /// <summary>
    /// Запись сериализации конкретного компонента в персональном JSON сущности.
    /// Хранит полное имя типа C# и его сериализованную JSON-строку.
    /// </summary>
    [Serializable]
    public class ComponentSaveEntry
    {
        public string TypeName;
        public string JsonData;
    }

    /// <summary>
    /// Сериализуемый контейнер полного состояния сущности в ее персональном файле {EntityId}.json на SSD.
    /// </summary>
    [Serializable]
    public class EntitySaveData
    {
        public string EntityId;
        public string TemplateId;
        public string LocationId;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsConfigured;
        public List<ComponentSaveEntry> Components = new();
    }

    /// <summary>
    /// Размороженная сущность в оперативной памяти (уровень L1).
    /// Чистый C# объект без привязки к GameObject сцены Unity.
    /// Управляет компонентами, их жизненным циклом и локальной шиной событий.
    /// </summary>
    public class EntityL1 : IEntity, IEntityRuntime, IDisposable
    {
        public string EntityId { get; private set; }
        public string TemplateId { get; private set; }
        public string SpawnerId { get; set; }
        public bool IsConfigured { get; set; }

        /// <summary>Физический скафандр (координаты, поворот, привязка L2/L3).</summary>
        public BodyComponent Body { get; set; } = new();

        /// <summary>Исходный дескриптор спящей записи L0.</summary>
        public EntityL0 L0 { get; set; }

        /// <summary>Локальная шина сигналов сущности для слабой связности компонентов.</summary>
        public EntityEventBus Events { get; } = new();

        public bool IsInitialized { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsDisposed { get; private set; }

        private readonly Dictionary<Type, IEntityComponent> _components = new();

        // Оптимизированные списки тактовых компонентов для быстрого вызова в Update-циклах
        private readonly List<IFixedUpdateComponent> _fixedUpdateComponents = new();
        private readonly List<IUpdateComponent> _updateComponents = new();
        private readonly List<ILateUpdateComponent> _lateUpdateComponents = new();
        private readonly List<IPauseUpdateComponent> _pauseUpdateComponents = new();

        public EntityL1(string entityId, string templateId = null)
        {
            EntityId = entityId;
            TemplateId = templateId;
            Body.BodyId = entityId;
        }

        public EntityL1(EntityL0 l0)
        {
            EntityId = l0.EntityId;
            TemplateId = l0.TemplateId;
            SpawnerId = l0.SpawnerId;
            IsConfigured = l0.IsConfigured;
            L0 = l0;

            Body = new BodyComponent(l0.EntityId, l0.LocationId, l0.LocalPosition, Quaternion.identity);
        }

        public void Setup()
        {
            IsConfigured = true;
        }

        public IEnumerable<IEntityComponent> GetAllComponents() => _components.Values;

        public T GetComponent<T>() where T : class, IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }

            // Поиск по производным типам/интерфейсам
            foreach (var comp in _components.Values)
            {
                if (comp is T typedComp)
                {
                    return typedComp;
                }
            }

            return null;
        }

        public void AddComponent(IEntityComponent component)
        {
            if (component == null) return;
            var type = component.GetType();
            _components[type] = component;

            RegisterTickInterfaces(component);

            if (IsInitialized && !component.IsInitialized)
            {
                component.Init(this);
            }

            if (IsStarted && !component.IsStarted)
            {
                component.Start();
            }
        }

        public void RemoveComponent<T>() where T : class, IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                if (component.IsStarted)
                {
                    component.Stop();
                }
                UnregisterTickInterfaces(component);
                _components.Remove(typeof(T));
                component.Dispose();
            }
        }

        // --- Фаза 2: Связывание зависимостей (Init) ---
        public void InitComponents()
        {
            if (IsDisposed || IsInitialized) return;
            IsInitialized = true;

            foreach (var component in _components.Values)
            {
                if (!component.IsInitialized)
                {
                    component.Init(this);
                }
            }
        }

        // --- Фаза 3: Запуск работы (Start) ---
        public void Start()
        {
            if (IsDisposed || IsStarted) return;
            if (!IsInitialized) InitComponents();

            IsStarted = true;

            foreach (var component in _components.Values)
            {
                if (!component.IsStarted)
                {
                    component.Start();
                }
            }
        }

        // --- Фаза 4: Остановка работы (Stop) ---
        public void Stop()
        {
            if (!IsStarted || IsDisposed) return;
            IsStarted = false;

            foreach (var component in _components.Values)
            {
                if (component.IsStarted)
                {
                    component.Stop();
                }
            }
        }

        // --- Загрузка и сериализация компонентов (Фаза 1 и Фаза 4) ---
        public void LoadComponents()
        {
            string sessionId = GlobalGame.Session?.SessionId;
            if (string.IsNullOrEmpty(sessionId)) return;

            string folder = DataPathManager.Entities(sessionId);
            var data = SaveData.Load<EntitySaveData>(folder, EntityId);
            if (data != null)
            {
                Load(data);
            }
        }

        public void SaveComponents()
        {
            string sessionId = GlobalGame.Session?.SessionId;
            if (string.IsNullOrEmpty(sessionId)) return;

            string folder = DataPathManager.Entities(sessionId);
            var data = Save();
            SaveData.Save(data, folder, EntityId);
        }

        public void Load(EntitySaveData data)
        {
            if (data == null) return;

            IsConfigured = data.IsConfigured;
            Body.Position = data.Position;
            Body.Rotation = data.Rotation;
            Body.LocationId = data.LocationId;

            var l0 = L0;
            l0.LocalPosition = data.Position;
            l0.LocationId = data.LocationId;
            l0.IsConfigured = data.IsConfigured;
            L0 = l0;

            // Фаза 1: Инстанцирование компонентов и передача им личных JSON данных
            if (data.Components != null)
            {
                foreach (var entry in data.Components)
                {
                    if (string.IsNullOrEmpty(entry.TypeName)) continue;

                    Type type = Type.GetType(entry.TypeName);
                    if (type != null && typeof(IEntityComponent).IsAssignableFrom(type))
                    {
                        try
                        {
                            var comp = (IEntityComponent)Activator.CreateInstance(type);
                            comp.Load(entry.JsonData);
                            _components[type] = comp;
                            RegisterTickInterfaces(comp);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"[EntityL1] Не удалось создать компонент {entry.TypeName}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[EntityL1] Тип компонента не найден: {entry.TypeName}");
                    }
                }
            }
        }

        public EntitySaveData Save()
        {
            var data = new EntitySaveData
            {
                EntityId = EntityId,
                TemplateId = TemplateId,
                LocationId = Body.LocationId,
                Position = Body.Position,
                Rotation = Body.Rotation,
                IsConfigured = IsConfigured
            };

            foreach (var comp in _components.Values)
            {
                string json = comp.Save();
                data.Components.Add(new ComponentSaveEntry
                {
                    TypeName = comp.GetType().AssemblyQualifiedName,
                    JsonData = json
                });
            }

            return data;
        }

        // --- Такты жизненного цикла ---
        public void FixedUpdate()
        {
            if (!IsStarted) Start();

            for (int i = 0; i < _fixedUpdateComponents.Count; i++)
            {
                _fixedUpdateComponents[i].FixedUpdate();
            }
        }

        public void Update()
        {
            if (!IsStarted) Start();

            for (int i = 0; i < _updateComponents.Count; i++)
            {
                _updateComponents[i].Update();
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < _lateUpdateComponents.Count; i++)
            {
                _lateUpdateComponents[i].LateUpdate();
            }
        }

        public void PauseUpdate()
        {
            for (int i = 0; i < _pauseUpdateComponents.Count; i++)
            {
                _pauseUpdateComponents[i].PauseUpdate();
            }
        }

        private void RegisterTickInterfaces(IEntityComponent comp)
        {
            if (comp is IFixedUpdateComponent f && !_fixedUpdateComponents.Contains(f)) _fixedUpdateComponents.Add(f);
            if (comp is IUpdateComponent u && !_updateComponents.Contains(u)) _updateComponents.Add(u);
            if (comp is ILateUpdateComponent l && !_lateUpdateComponents.Contains(l)) _lateUpdateComponents.Add(l);
            if (comp is IPauseUpdateComponent p && !_pauseUpdateComponents.Contains(p)) _pauseUpdateComponents.Add(p);
        }

        private void UnregisterTickInterfaces(IEntityComponent comp)
        {
            if (comp is IFixedUpdateComponent f) _fixedUpdateComponents.Remove(f);
            if (comp is IUpdateComponent u) _updateComponents.Remove(u);
            if (comp is ILateUpdateComponent l) _lateUpdateComponents.Remove(l);
            if (comp is IPauseUpdateComponent p) _pauseUpdateComponents.Remove(p);
        }

        public void Dispose()
        {
            if (IsDisposed) return;

            if (IsStarted)
            {
                Stop();
            }

            IsDisposed = true;

            foreach (var component in _components.Values)
            {
                component.Dispose();
            }

            _components.Clear();
            _fixedUpdateComponents.Clear();
            _updateComponents.Clear();
            _lateUpdateComponents.Clear();
            _pauseUpdateComponents.Clear();
            Events.Clear();
        }
    }
}
