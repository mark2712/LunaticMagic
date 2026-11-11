// using System;
// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;

// namespace Entities
// {
//     public enum RuntimeState
//     {
//         None,
//         Initialized,
//         Started,
//         Stopped,
//         Disposed
//     }

//     // Управляет компонентами
//     public interface IEntityRuntime : IDisposable
//     {
//         IEntity Entity { get; }
//         RuntimeState State { get; }

//         IEnumerable<IEntityComponent> GetAllComponents();
//         T GetComponent<T>() where T : class, IEntityComponent;
//         void AddComponent(IEntityComponent component);
//         void RemoveComponent<T>() where T : IEntityComponent;

//         void LoadComponents();
//         void SaveComponents();

//         // Unity методы компонентов
//         void FixedUpdate();
//         void PauseUpdate();
//         void Update();
//         void LateUpdate();
//     }

//     public class EntityRuntime : IEntityRuntime
//     {
//         public IEntity Entity { get; private set; }
//         public RuntimeState State { get; private set; } = RuntimeState.None;

//         private readonly Dictionary<Type, IEntityComponent> _components = new();
//         public IEnumerable<IEntityComponent> GetAllComponents() => _components.Values;
//         public string EntityPath => DataPathManager.Entities(GlobalGame.Session.SessionId);

//         private readonly List<IFixedUpdateComponent> _fixedUpdateComponents = new();
//         private readonly List<IPauseUpdateComponent> _pauseUpdateComponents = new();
//         private readonly List<IUpdateComponent> _updateComponents = new();
//         private readonly List<ILateUpdateComponent> _lateUpdateComponents = new();

//         private List<IFixedUpdateComponent> FixedUpdateComponents;
//         private List<IPauseUpdateComponent> PauseUpdateComponents;
//         private List<IUpdateComponent> UpdateComponents;
//         private List<ILateUpdateComponent> LateUpdateComponents;

//         public EntityRuntime(IEntity entity)
//         {
//             Entity = entity;
//         }

//         public T GetComponent<T>() where T : class, IEntityComponent
//         {
//             _components.TryGetValue(typeof(T), out var component);
//             return component as T;
//         }

//         public void AddComponent(IEntityComponent component)
//         {
//             _components[component.GetType()] = component;
//             if (component is IFixedUpdateComponent f) _fixedUpdateComponents.Add(f);
//             if (component is IPauseUpdateComponent p) _pauseUpdateComponents.Add(p);
//             if (component is IUpdateComponent u) _updateComponents.Add(u);
//             if (component is ILateUpdateComponent l) _lateUpdateComponents.Add(l);
//         }

//         public void RemoveComponent<T>() where T : IEntityComponent
//         {
//             if (_components.TryGetValue(typeof(T), out var component))
//             {
//                 if (component is IFixedUpdateComponent f) _fixedUpdateComponents.Remove(f);
//                 if (component is IPauseUpdateComponent p) _pauseUpdateComponents.Remove(p);
//                 if (component is IUpdateComponent u) _updateComponents.Remove(u);
//                 if (component is ILateUpdateComponent l) _lateUpdateComponents.Remove(l);
//                 _components.Remove(typeof(T));
//             }
//         }

//         private void Start()
//         {
//             if (State == RuntimeState.Started) return;
//             State = RuntimeState.Started;
//             FixedUpdateComponents = _fixedUpdateComponents;
//             PauseUpdateComponents = _pauseUpdateComponents;
//             UpdateComponents = _updateComponents;
//             LateUpdateComponents = _lateUpdateComponents;
//         }

//         private void Stop()
//         {
//             if (State == RuntimeState.Stopped) return;
//             State = RuntimeState.Stopped;
//             // отключаем update loops
//             FixedUpdateComponents = null;
//             PauseUpdateComponents = null;
//             UpdateComponents = null;
//             LateUpdateComponents = null;
//         }

//         public void Dispose()
//         {
//             if (State == RuntimeState.Disposed) return;
//             if (State == RuntimeState.Started) Stop();
//             State = RuntimeState.Disposed;
//             foreach (var component in _components.Values)
//             {
//                 component.Dispose();
//             }
//         }

//         public void LoadComponents()
//         {
//             // Метод вызывается только если сущность уже была настроена (IsConfigured == true)
//             var data = SaveData.Load<EntityComponentsData>(EntityPath, Entity.EntityId);

//             if (data == null)
//             {
//                 Debug.LogError($"[Entity] Попытка загрузить компоненты для настроенной сущности {Entity.EntityId}, но файл сохранения не найден!");
//                 return;
//             }

//             // 2. Инициализируем компоненты через рефлексию
//             foreach (var compData in data.Components)
//             {
//                 // Type.GetType требует полного имени с пространством имен
//                 Type type = Type.GetType(compData.TypeName);

//                 if (type != null && typeof(IEntityComponent).IsAssignableFrom(type))
//                 {
//                     var component = (IEntityComponent)Activator.CreateInstance(type);
//                     component.Load(compData.data);
//                     AddComponent(component);
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"[Entity] Не удалось загрузить компонент: {compData.TypeName}");
//                 }
//             }
//         }

//         public void SaveComponents()
//         {
//             if (State == RuntimeState.None) return;

//             // 1. Собираем данные со всех рантайм-компонентов
//             EntityComponentsData data = new();

//             foreach (var component in GetAllComponents())
//             {
//                 data.Components.Add(new ComponentSaveData
//                 {
//                     // AssemblyQualifiedName нужен, если компоненты лежат в разных сборках (Assembly)
//                     TypeName = component.GetType().AssemblyQualifiedName,
//                     data = component.Save() // Компонент сам сериализует свои данные
//                 });
//             }

//             // 2. Сохраняем в новый файл
//             SaveData.Save(data, EntityPath, Entity.EntityId);
//         }

//         public void FixedUpdate() { foreach (var component in FixedUpdateComponents) component.FixedUpdate(); }
//         public void PauseUpdate() { foreach (var component in PauseUpdateComponents) component.PauseUpdate(); }
//         public void Update() { foreach (var component in UpdateComponents) component.Update(); }
//         public void LateUpdate() { foreach (var component in LateUpdateComponents) component.LateUpdate(); }
//     }


//     [Serializable]
//     public class ComponentSaveData
//     {
//         public string TypeName; // Полное имя типа компонента для рефлексии
//         public object data; // Внутренние данные компонента
//     }
// }


// // public interface IEntityRuntime1 : IDisposable
// // {
// //     RuntimeState State { get; }

// //     void Init(); // загрузка компонентов
// //     void Start(); // запуск логики
// //     void Stop(); // остановка логики
// //     // void Dispose();  // сохранение + уничтожение

// //     // --- события lifecycle ---
// //     event Action OnInitialized;
// //     event Action OnStarted;
// //     event Action OnStopped;
// //     event Action OnDisposed;

// //     // --- события компонентов ---
// //     event Action<IEntityComponent> OnComponentAdded;
// //     event Action<IEntityComponent> OnComponentRemoved;
// // }


// // Init это new EntityRuntime, LoadComponents и AddComponent
// // void Start(); // старт всех компонентов (создание GO, включение Update, AI...)
// // void Stop(); // стоп всех компонентов (уничтожение GO, отключение Update, AI...)
// // void Dispose(); // стоп всех компонентов и уничтожение компонентов (и данных в них)

// // foreach (var component in _components.Values)
// // {
// //     component.Start(); // Ready
// // }

// // foreach (var component in _components.Values)
// // {
// //     component.Stop();
// // }