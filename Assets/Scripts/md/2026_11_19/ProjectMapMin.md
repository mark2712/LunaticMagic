# Карта проекта (Мини) — 19 ноября 2026

**Проект:** LunaticMagicV3  
**Папка документации:** `Assets/Scripts/md/2026_11_19/`  
**Предыдущая версия:** [ProjectMapMin.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/ProjectMapMin.md) (10 ноября 2026)

---

## 1. Система ресурсов (ResourceSystem) — Готово
* **Централизованный менеджер ресурсов:** [ResourceSystem.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/ResourceSystem.cs) (`ResourceManager`).
* **Каналы загрузки:**
  * **Addressables:** [AddressableManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/Controllers/Unity/Addressable/AddressableManager.cs) — основной асинхронный канал загрузки ассетов и бандлов.
  * **Локализация:** [LocalizationManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/Controllers/Localization/LocalizationManager.cs), [FileLocalizationManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/Controllers/Unity/File/FileLocalizationManager.cs), текстовые байндинги.
  * **VRM 1.0:** [VrmFileManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/Controllers/Unity/File/VrmFileManager.cs) — загрузка моделей персонажей из файлов.
  * **Resources (Legacy):** [ResourcesManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/Controllers/Unity/Resources/ResourcesManager.cs).
* **Очистка памяти:** [ClearManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/Clean/ClearManager.cs), [ClearManagerMemory.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/ResourceSystem/Clean/ClearManagerMemory.cs) — многоуровневая очистка кэшей (`CleanupLevel`).

---

## 2. Точка входа и Глобальные сервисы — Готово
* **Сцена `Init.unity` (индекс 0):** компонент [BootstrapGlobalGame.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/BootstrapGlobalGame.cs) (`DontDestroyOnLoad`).
* **Глобальный фасад:** [GlobalGame.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/GlobalGame.cs) — доступ к активной сессии, менеджеру ресурсов, настройкам и глобальному стейту UI.
* **Настройки и Сложность:** [GlobalGameSettings.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Global/GlobalGameSettings.cs), [DifficultyGame.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Global/Difficulty/DifficultyGame.cs).
* **Пути данных:** [DataPathManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/DataPathManager.cs) — централизованные пути к профилям, сейвам, сессиям, реестру сущностей и файлам настроек.

---

## 3. Сессия, Профили и Сохранения — Стабилизировано
* **Класс сессии:** [GameSession.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/GameSession.cs).
* **Изолированная песочница:** Рантайм-сессия живёт в `Sessions/{SessionId}/`. Сущности сохраняются в `Sessions/{SessionId}/Entities/`.
* **Загрузка сейва:** Содержимое сейва целиком копируется в папку активной сессии. Рантайм работает только в ней, не трогая исходный сейв.
* **Сохранение (Save):** Создаётся папка нового сейва в профиле (`Profiles/{ProfileId}/Saves/{SaveId}`), куда атомарно копируется состояние песочницы сессии.
* **Профили и Сейвы:** [GameProfiles.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Global/Profiles/GameProfiles.cs), [GameSave.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Global/Saves/GameSave.cs), [SaveData.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Common/Save/SaveData.cs).

---

## 4. Стейт-машина Сессии (SessionSM) и Загрузка — Готово
* **Менеджер состояний сессии:** [SessionStateManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/SM/SessionStateManager.cs).
  * Очередь событий `_eventQueue` с детерминированной обработкой в конце кадра (`LateUpdate()`).
  * Реактивный расчет текущего состояния через чистый вычислитель [SessionStateComputed.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/SM/SessionStateComputed.cs).
* **Четыре состояния сессии:**
  1. `SessionInitialLoadingState` — первичная загрузка сессии (наивысший приоритет, прогресс-бар, мир на паузе).
  2. `SessionEntitiesSyncState` — синхронизация сущностей (барьер при телепорте, смене локации или спавне, мир на паузе, блэкаут).
  3. `SessionPauseScreenState` — экран паузы меню (`Esc`), мир на паузе, управление у UI паузы.
  4. `SessionGameplayState` — активный геймплей (`Time.timeScale = 1`, симуляция тикает, управление у сущности в фокусе).
* **Загрузчик игры:** [GameLoader.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/Loading/GameLoader.cs) и [GameLoadingProgress.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/Loading/GameLoadingProgress.cs).
  * На текущем этапе эмулирует загрузку и синхронизацию сущностей через UniRx-таймер (`Scheduler.MainThreadIgnoreTimeScale`, 100 шагов по 20 мс) до создания полноценного барьера сущностей.
  * По завершении атомарно переводит сессию: `IsEntitiesSync = false`, `IsGameplay = true`, `IsSessionInitLoading = false`.
* **Управление курсором сессии:** [CursorState.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/Cursor/CursorState.cs) и [CursorComputed.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/Cursor/CursorComputed.cs) — реактивный расчет видимости и блокировки курсора (`Locked`, `VisibleConfined`, `VisibleNone`) в зависимости от состояния `SessionStateManager`.
* **Подсистема времени сессии:** [LocalGameTime.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/Time/LocalGameTime.cs) — разделение `FixedUpdate` (игровое время мира, пауза, шкала) и `Update` (независимое время паузы).
* **Подсистема звука сессии:** [SessionSoundController.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/Sound/SessionSoundController.cs) — буфер из 3-х слотов (`GameplayTrack`, `PauseTrack`, `LoadingTrack`), авто-реакция на стейт сессии, сохранение `PlaybackTime`.

---

## 5. Система UI (UITK & UniRx) — Готово
* **Инфраструктура компонентов:** [UIManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/UITK/UIManager.cs), [UIController.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/UITK/UIController.cs), [UIComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/UITK/UIComponent/UIComponent.cs), [ReactiveUIComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/UITK/UIComponent/ReactiveUIComponent.cs).
* **Трехслойная архитектура корня UI ([UIComponentRoot.cs](file:///D:/unity/LunaticMagicV3/Assets/MyFiles/UITK/Root/UIComponentRoot.cs)):**
  1. **Глобальный слой ([GlobalGameLayerComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/MyFiles/UITK/Root/GlobalGameLayerComponent.cs)):** Консоль (`~`), меню дебага, менеджер профилей и сейвов, главное меню.
  2. **Слой экранов сессии ([SessionLayerComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/MyFiles/UITK/Root/SessionLayerComponent.cs)):** Монтирует [SessionScreensComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/MyFiles/UITK/Session/SessionScreensComponent.cs), который реактивно отображает экраны сессии:
     * `InitialLoadingComponent` — прогресс первичной загрузки из `GameLoader`.
     * `EntitiesSyncComponent` — экран ожидания барьера пространственной синхронизации.
     * `PauseScreenComponent` — экран меню паузы.
  3. **Геймплейный слой ([GameplayLayerComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/MyFiles/UITK/Root/GameplayLayerComponent.cs)):** [GameScreenComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/MyFiles/UITK/Gameplay/GameScreen/GameScreenComponent.cs) — пассивный HUD, реактивно читающий данные сущности в фокусе.

---

## 6. Сущности и Стриминг (Текущий статус и Целевой план)
* **Текущее состояние в кодовой базе ([Assets/Scripts/Entities](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Entities/)):**
  * [EntitiesManager.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Entities/EntitiesManager.cs) — базовый реестр `_entities`, активных в памяти `_activeEntities` (L1), спавнеров `_spawnedEntities`. Методы `Activate(id)` (L0 $\rightarrow$ L1), `Deactivate(id)` (L1 $\rightarrow$ L0), `Save()`, `Load()`.
  * [Entity.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Entities/Entity.cs), [EntityRuntime.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Entities/EntityRuntime.cs), [EntityComponent.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Entities/EntityComponent.cs) — базовые контракты и стабы.
  * [IFocusable.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Entities/IFocusable.cs) — контракт фокуса с режимом курсора.
* **Главная задача текущего этапа:** Заменить таймер-заглушку в [GameLoader.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/Session/Loading/GameLoader.cs) на реальную систему сущностей и Барьер Пространственной Синхронизации (`SpatialSyncBarrier`).
* 👉 **Детальный архитектурный план системы сущностей:** [EntitySystemPlan.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_19/EntitySystemPlan.md).
* 👉 **Рабочий план по L0–L1, Стримингу и Дебаг L2:** [01_Entities_L0_L1_Streaming_Plan.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_19/01_Entities_L0_L1_Streaming_Plan.md).
* 👉 **Локации, Запекание и Барьер Синхронизации:** [02_Locations_And_Sync_Barrier.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_19/02_Locations_And_Sync_Barrier.md).
* 👉 **Компоненты L1, Жизненный Цикл и Взаимодействие:** [03_Components_L1_Lifecycle_And_Phases.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_19/03_Components_L1_Lifecycle_And_Phases.md).

---

## 7. Управление, Ввод и Оптимизация Тиков (Новые планы от 19.11.2026)
* **Промежуточный слой ввода (Input Routing & Buffering):**
  * Строгое разделение: Аппаратный драйвер (`InputLayer`) $\rightarrow$ Сессионный маршрутизатор и буфер (`InputRouter` / `InputBuffer`) $\rightarrow$ Сущность в фокусе (`EntityInputContext`).
  * Правило поглощения (Consumption): Стейт-машина сущности сжирает импульсы при совершении действия.
  * Защита от залипаний (Anti-Sticking): При смене фокуса или паузе сессии буфер мгновенно сбрасывает удержания и таймеры.
  * 👉 **Спецификация:** [InputPlan.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_19/InputPlan.md).
* **Планировщик периодических задач FixedUpdate (Zero-GC Hashed Timing Wheel):**
  * Решение проблемы пиковых нагрузок процессора при регенерации (HP, мана, стамина, редкий ИИ).
  * Кольцевой массив на 1024 корзины со случайным фазовым сдвигом при спавне и джиттером при каждом тике.
  * 0 байт GC, равномерное распределение задач по кадрам FixedUpdate.
  * 👉 **Спецификация:** [planFU.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_19/planFU.md).

---

## 8. Сюжет, Квесты и Три Подхода к Триггерам
* Статусы и условия квестов (`IQuest`, `QuestsManager`).
* Локальные триггеры vs сюжетные проверки (квалификация подошедшего актора, опрос источника, прямое изменение L1).
* При вселении в чужое тело (`Possession`) физические триггеры срабатывают на тело, а сюжетные фильтруются по `PrimaryEntityId` мага.
* 👉 **Базовая спецификация:** [StoryAndQuests.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/StoryAndQuests/StoryAndQuests.md).
