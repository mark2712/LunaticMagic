// using System.Collections.Generic;
// using UniRx;

// namespace UITK
// {
//     public class Navigation
//     {
//         public ReactiveProperty<IUINode> FocusedNode;
//         public IUINode Root;
//         public IUINode GetPathNode(string path)
//         {
//             // return 
//         }

//         public void HandleEvent(string path, IUIEvent evt)
//         {

//         }
//     }

//     public abstract class UINode
//     {
//         public ReactiveProperty<bool> IsFocused { get; set; } // == Navigation.FocusedNode
//         public ReactiveProperty<bool> IsActive { get; set; } // Node находится на пути к FocusedNode
//         public ReactiveProperty<bool> IsOpen { get; set; }
//         public virtual void OnActive() { }
//         public virtual void OnDisable() { }
//         public virtual void OnOpen() { }
//         public virtual void OnClose() { }
//     }




//     public interface IUINode : IDisposable
//     {
//         string Id { get; }
//         IUINode Parent { get; }
//         IReadOnlyReactiveCollection<IUINode> Children { get; }

//         // Состояние
//         IReadOnlyReactiveProperty<bool> IsFocused { get; }
//         IReadOnlyReactiveProperty<bool> IsVisible { get; }

//         // Жизненный цикл (UniRx)
//         // Мы используем Disposable для закрытия, но эти методы нужны для логики входа
//         void OnInitialize();
//     }

//     // Для вкладок (Tabs): активен только один ребенок
//     public interface ISwitchContainer : IUINode
//     {
//         IReactiveProperty<IUINode> ActiveChild { get; }
//         void SwitchTo(string nodeId);
//     }

//     // Для стека (Popups/Settings): наложение друг на друга
//     public interface IStackContainer : IUINode
//     {
//         void Push(IUINode node);
//         void Pop();
//     }

//     public interface IUIEvent { } // Маркерный интерфейс для твоих событий (Esc, Apply, Close)

//     public interface IUIEventHandler
//     {
//         // Возвращает true, если событие поглощено
//         bool HandleEvent(IUIEvent evt);
//     }

//     public interface INavigationService
//     {
//         IUINode Root { get; }

//         // Свойство, на которое подписывается Input Manager
//         IReadOnlyReactiveProperty<IUINode> FocusedNode { get; }

//         // Навигация по путям: "MainMenu/Settings/Graphics"
//         void NavigateTo(string path);

//         // Возврат назад по иерархии (умный Esc)
//         void GoBack();
//     }

//     public class MainUI : UINode
//     {
//         public List<UINode> Nodes = new()
//         {
//             new HUD(),
//             new MainMenu(),
//             new PauseMenu()
//         };

//         public MainUI()
//         {
//             Nodes.Add(new HUD());
//         }
//     }

//     public class MainMenu : UINode
//     {
//         public List<UINode> Nodes = new()
//         {
//             new Settings()
//         };
//     }

//     public class PauseMenu : UINode
//     {
//         public List<UINode> Nodes = new()
//         {
//             new SaveLoad(),
//             new Settings()
//         };
//     }

//     public class HUD : UINode
//     {
//         public List<UINode> Nodes = new()
//         {

//         };
//     }


//     public class SaveLoad : UINode
//     {
//         public List<UINode> Nodes = new()
//         {
//             new PopupApply()
//         };
//     }

//     public class Settings : UINode
//     {
//         public List<UINode> Nodes = new()
//         {
//             new SettingsTabs(),
//             new PopupApply()
//         };

//         public class SettingsTabs
//         {
//             public List<UINode> Nodes = new()
//             {
//                 new Tab1()
//             };

//             public class Tab1
//             {

//             }
//         }
//     }

//     public class PopupApply : UINode
//     {
//         public List<UINode> Nodes = new()
//         {

//         };
//     }







//     // public interface IUINode1
//     // {
//     //     ReactiveCollection<IUINode> Nodes { get; }
//     //     ReactiveProperty<bool> IsActive { get; }
//     // }

//     // public interface IUILayer
//     // {
//     //     ReactiveProperty<bool> IsOpen { get; }
//     //     void Open();
//     //     void Close();
//     // }

//     // public interface IUINodeTabs : IUILayer
//     // {
//     //     ReactiveCollection<IUINodeTab> Tabs { get; }
//     //     ReactiveProperty<IUINodeTab> NowTab { get; }
//     //     void Set(IUINodeTab tab);
//     //     void CreateTab(IUINodeTab tab);
//     //     void CloseTab(IUINodeTab tab);
//     // }

//     // public interface IUINodeTab : IUINode { }

//     // public interface IUINodeWindows : IUILayer
//     // {
//     //     ReactiveCollection<IUINodeWindow> Windows { get; }
//     //     void CreateWindow(IUINodeWindow window);
//     //     void CloseWindow(IUINodeWindow window);
//     // }

//     // public interface IUINodeWindow : IUINode { }


//     // public interface IUINode
//     // {
//     //     ReactiveCollection<IUINode> Nodes { get; } // внутренние ноды
//     //     ReactiveProperty<bool> IsActive { get; } // это окно сейчас открыто (тут я думаю лучше вообще вынести это на верхний уровень чтобы было только одно активное окно как в ОС виндовс)
//     //     void Open(); // показать UI (может быть открыто сразу много UI)
//     //     void Close(); // скрыть UI
//     // }

//     // public class Navigation
//     // {
//     //     ReactiveProperty<IUINode> Active { get; }
//     // }
// }
