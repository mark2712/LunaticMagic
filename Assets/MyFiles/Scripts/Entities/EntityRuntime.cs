using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Entities
{
    // Управляет компонентами
    public interface IEntityRuntime : IDisposable
    {
        IEnumerable<IEntityComponent> GetAllComponents();
        T GetComponent<T>() where T : class, IEntityComponent;
        void AddComponent(IEntityComponent component);
        void RemoveComponent<T>() where T : IEntityComponent;

        // Init это new EntityRuntime
        void Start(); // старт всех компонентов (создание GO, включение Update, AI...)
        void Stop(); // стоп всех компонентов (уничтожение GO, отключение Update, AI...)
        // void Dispose(); // стоп всех компонентов и уничтожение компонентов (и данных в них)

        void LoadComponents(IEntity entity);
        public void SaveComponents(IEntity entity);

        // Unity методы компонентов
        void FixedUpdate();
        void PauseUpdate();
        void Update();
        void LateUpdate();
    }

    public class EntityRuntime : IEntityRuntime
    {
        private readonly Dictionary<Type, IEntityComponent> _components = new();
        public IEnumerable<IEntityComponent> GetAllComponents() => _components.Values;

        public bool IsStart { get; private set; }
        public bool IsDispose { get; private set; }

        private readonly List<IFixedUpdateComponent> _fixedUpdateComponents = new();
        private readonly List<IPauseUpdateComponent> _pauseUpdateComponents = new();
        private readonly List<IUpdateComponent> _updateComponents = new();
        private readonly List<ILateUpdateComponent> _lateUpdateComponents = new();

        private List<IFixedUpdateComponent> FixedUpdateComponents;
        private List<IPauseUpdateComponent> PauseUpdateComponents;
        private List<IUpdateComponent> UpdateComponents;
        private List<ILateUpdateComponent> LateUpdateComponents;


        public T GetComponent<T>() where T : class, IEntityComponent
        {
            _components.TryGetValue(typeof(T), out var component);
            return component as T;
        }

        public void AddComponent(IEntityComponent component)
        {
            _components[component.GetType()] = component;
            if (component is IFixedUpdateComponent f) _fixedUpdateComponents.Add(f);
            if (component is IPauseUpdateComponent p) _pauseUpdateComponents.Add(p);
            if (component is IUpdateComponent u) _updateComponents.Add(u);
            if (component is ILateUpdateComponent l) _lateUpdateComponents.Add(l);
        }

        public void RemoveComponent<T>() where T : IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                if (component is IFixedUpdateComponent f) _fixedUpdateComponents.Remove(f);
                if (component is IPauseUpdateComponent p) _pauseUpdateComponents.Remove(p);
                if (component is IUpdateComponent u) _updateComponents.Remove(u);
                if (component is ILateUpdateComponent l) _lateUpdateComponents.Remove(l);
                _components.Remove(typeof(T));
            }
        }

        public void Start()
        {
            if (IsStart) return;
            IsStart = true;
            foreach (var component in _components.Values)
            {
                component.Start();
            }
            FixedUpdateComponents = _fixedUpdateComponents;
            PauseUpdateComponents = _pauseUpdateComponents;
            UpdateComponents = _updateComponents;
            LateUpdateComponents = _lateUpdateComponents;
        }

        public void Stop()
        {
            if (!IsStart) return;
            IsStart = false;
            foreach (var component in _components.Values)
            {
                component.Stop();
            }
            // отключаем update loops
            FixedUpdateComponents = null;
            PauseUpdateComponents = null;
            UpdateComponents = null;
            LateUpdateComponents = null;
        }

        public void Dispose()
        {
            if (IsDispose) return;
            IsDispose = true;
            foreach (var component in _components.Values)
            {
                component.Dispose();
            }
        }

        public void LoadComponents(IEntity entity)
        {
            // 1. Пытаемся загрузить данные
            var data = SaveData.Load<EntityComponentsData>(DataPathManager.Entities, entity.EntityId);

            if (data != null)
            {
                // 2. Инициализируем компоненты через рефлексию
                foreach (var compData in data.Components)
                {
                    // Type.GetType требует полного имени с пространством имен
                    Type type = Type.GetType(compData.TypeName);

                    if (type != null && typeof(IEntityComponent).IsAssignableFrom(type))
                    {
                        var component = (IEntityComponent)Activator.CreateInstance(type);
                        component.Load(compData.JsonData); // Компонент сам парсит свой JSON
                        AddComponent(component);
                    }
                    else
                    {
                        Debug.LogWarning($"[Entity] Не удалось загрузить компонент: {compData.TypeName}");
                    }
                }

                // 3. УДАЛЯЕМ ФАЙЛ. Данные теперь в рантайме. 
                string filePath = Path.Combine(DataPathManager.Entities, entity.EntityId + ".json"); // Уточни расширение файла
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public void SaveComponents(IEntity entity)
        {
            if (!entity.IsActive) return;

            // 1. Собираем данные со всех рантайм-компонентов
            EntityComponentsData data = new();

            foreach (var component in GetAllComponents())
            {
                data.Components.Add(new ComponentSaveData
                {
                    // AssemblyQualifiedName нужен, если компоненты лежат в разных сборках (Assembly)
                    TypeName = component.GetType().AssemblyQualifiedName,
                    JsonData = component.Save() // Компонент сам сериализует свои данные
                });
            }

            // 2. Сохраняем в новый файл
            SaveData.Save(data, DataPathManager.Entities, entity.EntityId);
        }

        public void FixedUpdate() { foreach (var component in FixedUpdateComponents) component.FixedUpdate(); }
        public void PauseUpdate() { foreach (var component in PauseUpdateComponents) component.PauseUpdate(); }
        public void Update() { foreach (var component in UpdateComponents) component.Update(); }
        public void LateUpdate() { foreach (var component in LateUpdateComponents) component.LateUpdate(); }
    }


    [Serializable]
    public class ComponentSaveData
    {
        public string TypeName; // Полное имя типа компонента для рефлексии
        public string JsonData; // Внутренние данные компонента
    }
}