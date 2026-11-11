# Подсистема: Точка входа и Глобальные настройки (Entry Point)

**Файлы в коде:**  
- [GlobalGame.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/GlobalGame.cs#L92-L97)  
- [BootstrapGlobalGame.cs](file:///D:/unity/LunaticMagicV3/Assets/Scripts/Game/BootstrapGlobalGame.cs)  
**Папка документации:** `Assets/Scripts/md/2026_11_10/EntryPoint/`

---

## 1. Архитектура запуска

* **Сцена `Init.unity`:**  
  Стартовая сцена (индекс 0 в Build Settings). Содержит объект `InitGlobalGame` с компонентом `BootstrapGlobalGame`.
* **Паттерн `DontDestroyOnLoad`:**  
  Инициализирует глобальные менеджеры, загружает глобальные настройки и переводит игру в состояние готовности.
* **Сцены в разработке vs Рантайм:**  
  В рантайме игровой мир функционирует как единое бесшовное пространство. Однако для удобства разработки, тестирования и запекания каждый уровень/зона создаётся на своей отдельной сцене Unity.

---

## 2. Глобальные настройки (`GlobalGameData`)

* Сохраняются и загружаются независимо от игровых сессий (`DataPathManager.GlobalSaves`, файл `GlobalGameData.json`).
* Содержат:
  * `GlobalGameSettings` (графика, звук, системные флаги).
  * `Language` (текущий выбранный язык локализации).
