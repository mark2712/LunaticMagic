# Карта проекта (Мини) — 10 ноября 2026

**Проект:** LunaticMagicV3  
**Папка документации:** `Assets/Scripts/md/2026_11_10/`

---

## 1. Система ресурсов
* Централизованный менеджер ресурсов (`ResourceSystem`).
* Addressables (основной канал загрузки ассетов), локализация из файлов, специализированный канал VRM 1.0 (в разработке), устаревший канал Resources.
* 👉 **Подробная спецификация:** [ResourceSystem.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/ResourceSystem/ResourceSystem.md).

---

## 2. Точка входа и Глобальные настройки
* Сцена `Init.unity` (индекс 0), объект `InitGlobalGame`, компонент `BootstrapGlobalGame` (`DontDestroyOnLoad`).
* В рантайме игра работает бесшовно; в редакторе каждый уровень расположен на своей сцене.
* Общие настройки игры и язык (`GlobalGame.cs`, `GlobalGameData`).
* 👉 **Подробная спецификация:** [EntryPoint.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/EntryPoint/EntryPoint.md).

---

## 3. Сессия, Профили и Сохранения
* **Изолированная песочница:** работа сессии происходит в папке `Sessions/{SessionId}/`. Сущности хранятся внутри сессии в папке `Entities/`.
* **Загрузка:** содержимое папки сейва целиком копируется в папку текущей сессии; рантайм работает только в ней.
* **Сохранение:** создаётся новая папка сейва, куда копируется папка сессии со всеми сущностями.
* **Стейт сессии:** системный цикл `Loading` и `Pause` (не сериализуются, пауза морозит `LocalGameTime` и физику).
* 👉 **Подробная спецификация:** [SessionArchitecture.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/Session/SessionArchitecture.md) и [04_LoadingAndSpatialSyncBarrier.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/04_LoadingAndSpatialSyncBarrier.md).

---

## 4. Сущности и Стриминг
* **Пять уровней сущности:**
  * L0: Спит на диске (есть запись в словаре сессии: ID, координаты, размер).
  * L1: Загружена в память (чистый C# объект `EntityRuntime`, данные, A-Life симуляция вне физики).
  * L2: Замороженное физ-тело (isKinematic = true, коллайдер).
  * L3: Активная физика (isKinematic = false, NavMesh, локомоция).
  * L4: Визуальная модель (VRM/Mesh, Animator).
* **Зона ответственности Низкого Уровня:** перевод L0 $\rightarrow$ L1 через `CullingGroup` по виртуальному размеру, контроль вменяемого числа сущностей в памяти (~100 из 10 000). Не касается коллайдеров и физики.
* **Высокий уровень:** перевод L1 $\rightarrow$ L2..L4, макрогорода (подмена LOD, фасады) и синхронные механизмы (платформы).
* **Стриминг:** обычный плавный инкрементальный vs барьерный (с дожиданием ответа готовности сущностей при прыжке фокуса > X).
* 👉 **Подробная спецификация:** [EntitiesAndStreaming.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/Entities/EntitiesAndStreaming.md), [02_LowLevelStreamingAndEntities.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/02_LowLevelStreamingAndEntities.md) и [04_LoadingAndSpatialSyncBarrier.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/04_LoadingAndSpatialSyncBarrier.md).

---

## 5. Система Фокуса и Виртуальные Камеры
* **Чистый контракт фокуса (`IFocusable`):** `OnFocusGained()` и `OnFocusLost()`. Правило: сначала `entity.Setup(...)`, затем чистый `SetFocus(entity)`.
* **Виртуальные камеры сущностей:** глобального менеджера/стейта камер нет. У каждой сущности свои виртуальные камеры (0..N). Переключение 1-е/3-е лицо — локальная забота сущности.
* Попадание в туман, воду, вселение в рыбу, смерть или катсцена — это локальные свойства активной сущности или чистая смена фокуса.
* 👉 **Подробная спецификация:** [FocusAndCameras.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/FocusAndCameras/FocusAndCameras.md).

---

## 6. Управление, Ввод и ИИ (Командная система)
* Чёткое разделение: «Источник команд» (`PlayerBrain`, `AIBrain`) $\rightarrow$ «Приёмник команд» (`ICommandReceiver`).
* Единая природа ввода: игрок и ИИ равноправны, шлют абстрактные команды (`Move`, `Look`, `ExecuteAction(slot)`).
* Сущность сама мапит команды на свою локомоцию и слоты действий.
* 👉 **Подробная спецификация:** [CommandSystem.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/InputAndAI/CommandSystem.md).

---

## 7. Система UI (UITK & UniRx)
* Пассивный исполнитель внизу стека: не имеет своего геймплейного стейта, реактивно отображает стейт сущности в фокусе.
* Контракт `IEntityUI`: отображение HUD при получении фокуса, закрытие окон при потере фокуса.
* Файлы UI строго в `Assets/MyFiles/UITK/`.
* 👉 **Подробная спецификация:** [UISystem.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/UI/UISystem.md).

---

## 8. Сюжет, Квесты и Три Подхода к Триггерам
* Статусы и условия квестов (`IQuest`, `QuestsManager`).
* **Три подхода к сюжетным моментам (на примере кнопки и двери):**
  1. *Прямое изменение состояния:* разморозить дверь из L0 в L1 и сменить её состояние навсегда.
  2. *Опрос источника:* кнопка меняет свой стейт, а дверь при приближении опрашивает кнопку.
  3. *Квалификация актора:* кнопка делает запись в подошедшего (бафф, флаг, журнал), а дверь опрашивает подошедшего (магу открыто, орку заперто).
* 👉 **Подробная спецификация:** [StoryAndQuests.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/StoryAndQuests/StoryAndQuests.md) и [03_SessionEntitiesAndStoryState.md](file:///D:/unity/LunaticMagicV3/Assets/Scripts/md/2026_11_10/03_SessionEntitiesAndStoryState.md).
