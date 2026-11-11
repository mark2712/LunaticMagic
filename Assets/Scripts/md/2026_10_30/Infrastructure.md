# Чеклист Развертывания, Инфраструктуры и Восстановления (Setup & Recovery Guide)

**Проект:** LunaticMagic  
**Компания:** MadLun  

> **Назначение документа:** Руководство на случай переустановки проекта, смены версии Unity, клонирования из Git или сбоя ассетов. Инструкция, как проверить и починить всё, что обычно слетает.

---

## 1. Чеклист зависимостей (Если проект не компилируется)

При открытии чистого проекта убедиться, что установлены все обязательные пакеты:

### В Package Manager (Window -> Package Manager):
* **Addressables** (`com.unity.addressables` 2.9.1) — система асинхронной загрузки.
* **Input System** (`com.unity.inputsystem` 1.20.0) — система ввода.
* **Newtonsoft JSON** (`com.unity.nuget.newtonsoft-json` 3.2.2) — сериализация сейвов.

### В папке `Assets/Plugins/`:
* **UniRx** (`Assets/Plugins/UniRx/`) — без него упадет вся реактивность UI и геймплея.
* **UniVRM / VRM 1.0** — без него не скомпилируется загрузка и анимация аватаров L4.

---

## 2. Настройки Player Settings (Если слетели сохранения)

Если переустановить проект или обновить редактор, Unity сбрасывает имя компании на `DefaultCompany`. В итоге игра создает новую пустую папку, а старые сохранения и профили теряются!

### Что проверить:
1. Открыть: `Edit -> Project Settings -> Player`.
2. Проверить поля:
   * **Company Name:** строго `MadLun`
   * **Product Name:** строго `LunaticMagic`
3. Путь к сейвам на ПК должен быть строго:  
   `%USERPROFILE%/AppData/LocalLow/MadLun/LunaticMagic/`

---

## 3. Сцена `Init` и сбитый скрипт на `BootstrapGlobalGame`

Если перемещались папки со скриптами или пересоздавалась сцена, на стартовом объекте может появиться критическая ошибка **`Missing (Mono Script)`**. Игра запустится, но будет висеть вечный пустой экран.

### Что проверить:
1. Открыть сцену: **`Assets/Scenes/System/Init.unity`**.
2. Выбрать в иерархии объект: **`InitGlobalGame`**.
3. Убедиться, что на объекте висит компонент **`BootstrapGlobalGame`** (нет желтых надписей `Missing Script`). Если сбился — заново перетянуть `Assets/Scripts/Game/BootstrapGlobalGame.cs`.
4. Открыть: `File -> Build Settings`.
5. Убедиться, что сцена `Init.unity` добавлена в список Scenes in Build и стоит **строго под номером 0**.

---

## 4. Папка `Assets/MyFiles/UITK` (Если пропал интерфейс)

Папка UI зафиксирована по строгому пути. Если переместить её в другое место, ссылки в панелях и коде слетают!

### Правило расположения:
* Все файлы интерфейса лежат **СТРОГО** в: **`Assets/MyFiles/UITK/`**.
* Внутри находятся:
  * `UITKPanelSettings.asset` (главные настройки панели UI Toolkit).
  * `Root/RootUIDocument.prefab` (базовый документ интерфейса).
  * `Menu/`, `Inventory/`, `Dialogs/`, `Styles/`, `Components/`.

### Что делать, если пути к UI слетели:
1. Вернуть папку на место: `Assets/MyFiles/UITK/`.
2. Проверить в префабе `RootUIDocument`, что поле `Panel Settings` ссылается на `Assets/MyFiles/UITK/UITKPanelSettings.asset`.

---

## 5. Addressables: если ресурсы пропали или не грузятся

Если файлы перемещались по проекту, в Addressables они могут стать `Missing` или потерять адреса. В рантайме игра упадет с ошибкой загрузки ассета.

### Чеклист проверки Addressables:
1. Все динамические ресурсы лежат в: **`Assets/MyFiles/Addressables/`** (`Characters/`, `Locations/`, `Audio/`, `Data/`).
2. Открыть окно: `Window -> Asset Management -> Addressables -> Groups`.
3. Убедиться, что в группах нет красных битых записей (Missing).
4. Проверить имена адресов (Address) — они не должны содержать длинных локальных путей диска, а иметь вид:
   * `vrm/player_default`
   * `loc/forest_chunk`
5. **Обязательно пересобрать бандлы:**
   * В окне Groups нажать: `Build -> New Build -> Default Build Script`.
