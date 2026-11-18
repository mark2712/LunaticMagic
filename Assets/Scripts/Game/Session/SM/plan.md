# План архитектуры Session State Machine (SessionSM)

## 1. Цели и мотивация

1. **Изоляция уровней игры**: 
   - Разделение нижнего уровня игры (жизненный цикл сессии: загрузка, пауза, синхронизация сущностей) и верхнего уровня (сущности, инвентарь, анимации, руки/ноги персонажа).
   - Для сессии создаётся собственная, независимая и легковесная машина состояний (`SessionSM`).
2. **Устранение проблем синхронного переключения (Input Bouncing / Race Conditions)**:
   - В текущей реализации прямое изменение флага `IsGameplay.Value = false` посреди обработки ввода вызывает моментальную смену стейта и включение другого роутера ввода в том же кадре, из-за чего зажатая клавиша срабатывает дважды.
   - Решение: **Очередь событий (Event Queue)** с отложенной обработкой в конце кадра (`LateUpdate`).
3. **Строгая инкапсуляция побочных эффектов**:
   - Внешние источники (кнопки, загрузчики, сетевые триггеры) **не имеют права** напрямую менять стейт сессии или запускать побочные эффекты.
   - Они лишь отправляют события в очередь: `SessionSM.Enqueue(SessionEvent.Pause)`.
   - Только активное состояние (`CurrentState`) решает:
     - Реагировать ли на это событие вообще (например, `Pause` во время загрузки просто игнорируется).
     - В какое новое состояние перейти.
     - Какие побочные эффекты выполнить (или они выполняются декларативно в `Enter()` / `Exit()`).

---

## 2. Основные концепции

### 2.1. Очередь событий и выполнение в конце кадра
```
[Input / GameLoader / External System]
                │
                ▼ (Enqueue)
       ┌─────────────────┐
       │   Event Queue   │  (хранит события текущего кадра)
       └─────────────────┘
                │
                ▼ (Tick / ProcessQueue в LateUpdate)
    ┌─────────────────────────┐
    │ CurrentState.OnEvent()  │
    └─────────────────────────┘
                │
                ├──> [Новый стейт] ──> Exit() старого ──> Enter() нового
                └──> [Побочные действия внутри состояния]
```
- Любое внешнее действие только помещает событие в очередь.
- В конце кадра (`LateUpdate` или подписка UniRx на конец кадра) очередь разгребается.
- Смена активного роутера ввода происходит **после** того, как текущий кадр ввода уже полностью завершился. Никакого дребезга и повторного срабатывания клавиш.

### 2.2. Новый `RegisterEvent`
В каждом состоянии события регистрируются явно через декларативную таблицу:
```csharp
public class SessionGameplayState : SessionStateBase
{
    public SessionGameplayState()
    {
        // Переход в паузу
        RegisterEvent(SessionEvent.Pause, () => SM?.PauseScreen);
        
        // Начало синхронизации мира/чанков
        RegisterEvent(SessionEvent.StartEntitiesSync, () => SM?.EntitiesSync);
    }
}
```
Преимущества:
- Если событие не зарегистрировано для стейта (например, `Pause` во время `InitialLoading`), оно безопасно игнорируется без `if/else` проверок.
- Обработчик может вернуть новый `SessionStateBase` (переход) или `null` (просто действие без перехода).

### 2.3. Побочные эффекты (Side Effects)
- **Только стейт управляет своими эффектами**:
  - `Enter()`: включение звукового слота, установка `Time.timeScale`, активация соответствующего `InputRouter`, блокировка/разблокировка курсора.
  - `Exit()`: очистка подписок, остановка звука, сброс временных модификаторов.
- Внешний код не «настраивает сессию под стейт» — стейт сам настраивает окружение при входе.

---

## 3. Предлагаемая структура файлов

```
Assets/Scripts/Game/Session/SM/
├── plan.md                          // Этот план
├── SessionEvent.cs                  // Enum всех событий уровня сессии (Pause, Resume, etc.)
├── PauseComputed.cs                 // Чистый вычислитель состояния сессии
├── SessionSM.cs                     // Машина состояний: очередь, ProcessQueue (LateUpdate), CurrentState
├── SessionStateBase.cs              // Базовый стейт: RegisterEvent, Enter, Exit, имя, реактивные свойства
└── States/
    ├── SessionInitialLoadingState.cs // Загрузка сейва/мира
    ├── SessionEntitiesSyncState.cs   // Синхронизация чанков/сущностей
    ├── SessionGameplayState.cs       // Геймплей (фокус курсора, игровой ввод)
    └── SessionPauseScreenState.cs    // Меню паузы
```

---

## 4. Что делать с существующей папкой `Assets/Scripts/Game/Session/State`

В папке `Assets/Scripts/Game/Session/State` уже написана и отлажена вся предметная логика состояний:
- `CursorMode` и логика фокуса сущностей (`IFocusable`) в `SessionStateGameplay`.
- Конфигурации времени (`TimeStateFactory.Pause()` vs `TimeStateFactory.Normal()`).
- Звуковые слоты (`SessionSoundSlot.Loading`, `Pause`, `Gameplay`).
- Реактивный `CurrentState` для UITK экранов.

**Решение**:
1. Написать чистый `SessionSM` в папке `SM/` с новыми парадигмами (очередь, отложенное исполнение, `RegisterEvent`).
2. Взять из `Assets/Scripts/Game/Session/State/SessionState.cs` готовую проверенную логику (звук, время, курсор) и перенести в новые классы `States/Session*.cs`.
3. Заменить `SessionStateManager` в `GameSession.cs` на `SessionSM` (сохранив для внешних потребителей, таких как UITK и `CursorState`, привычный контракт `IReadOnlyReactiveProperty<SessionStateBase> CurrentState`).
4. После успешного тестирования и перехода удалить устаревший `SessionStateManager` и `PauseComputed`.

---

## 5. Пошаговые этапы реализации

1. **Этап 1: Базовая структура SM**
   - Создать `SessionEvent.cs` (перечисление `InitialLoadingFinished`, `StartEntitiesSync`, `FinishEntitiesSync`, `Pause`, `Resume`).
   - Создать `PauseComputed.cs` (чистый вычислитель актуального состояния).
   - Создать `SessionStateBase.cs` с таблицей обработчиков `RegisterEvent(SessionEvent, Func<SessionStateBase>)`.
   - Создать `SessionSM.cs` с очередью `Queue<SessionEvent>`, методами `Enqueue()` и `ProcessQueue()` в LateUpdate.

2. **Этап 2: Конкретные состояния**
   - Реализовать `SessionInitialLoadingState`, `SessionEntitiesSyncState`, `SessionGameplayState`, `SessionPauseScreenState` на базе логики из текущего `SessionState.cs`.

3. **Этап 3: Интеграция в GameSession**
   - Подключить `SessionSM` к кадровому циклу Unity (`LateUpdate`).
   - Подключить роутеры ввода (`InputRouterGameplay` к `SessionEvent.Pause`, `InputRouterPause` к `SessionEvent.Resume` через `Enqueue()`).
   - Адаптировать `CursorState` и UITK (`SessionScreensComponent`) к новому `CurrentState`.

4. **Этап 4: Верификация и тестирование**
   - Проверить нажатие клавиши паузы по одной и той же кнопке (никаких двойных переключений).
   - Проверить цепочку: InitialLoading -> EntitiesSync -> Gameplay -> Pause -> Gameplay.
