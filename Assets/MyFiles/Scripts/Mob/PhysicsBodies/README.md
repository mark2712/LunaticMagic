# Управление физическими телами мобов и определение среды

- **PhysicsBodyController** — управление текущим физическим телом и подписка на детектор среды у тела. Смена тела на основе правил в `IEnvironmentBodySwitcher` при изменении среды.
- **IEnvironmentBodySwitcher** — решает, какое тело `IPhysicsBody` использовать на основе данных о текущей среде `EnvironmentData`. По сути тут находтся список тел моба (детекторы встроены в тела).
- **IPhysicsBody** — физический контроллер с физическим телом, имеет логику перемещения, устанавливает физическое поведение на сцене через `IScenePhysicsBody`, имеет собственный детектор среды `IEnvironmentDetector` и сам решает как сканировать окружающую среду.
- **IEnvironmentDetector** — сканирует окружающую среду и сообщает изменения. Формирует `EnvironmentData`. Тело `IPhysicsBody` сразу имеет свой детектор среды.
- **IScenePhysicsBody** — интерфейс для реальных Unity-компонентов на сцене. Доступ к `Rigidbody` и установка реального физ тела (GO, коллайдеры, физ материалы и тд).


## Расширение

1. **Environment/Detectors** - детекторы для разных мобов и тел.

2. **Mob/PhysicsBodies/Bodies** - физические тела, у каждого соба может быть много разных тел для разных ситуаций.

3. **Mob/PhysicsBodies/BodySwitchers** - решает какое физическое ткло выбрать на основе `EnvironmentData`, у каждого типа моба есть только один BodySwitcher.


## Как работает
### Как работает FixedUpdate
У моба в FixedUpdate происходит PhysicsBodyController.FixedUpdate(); \
У PhysicsBodyController происходит PhysicsBody.FixedUpdate(); \
У PhysicsBody происходит
1. сканирование среды EnvironmentDetector.FixedUpdate();
2. выполнение логики работы физики (перемещение персонажа и тд)

### Как происходит реакция на изменение среды
Создаём PhysicsBodyController и передаём в него IEnvironmentBodySwitcher и EnvironmentData из сохранения. Получаем физическое тело. \
Далее подписываемся на детектор из тела `EnvironmentDetector.OnEnvironmentChanged += OnEnvironmentChanged;` \
Теперь при изменении среды (среда сканируется в PhysicsBody.FixedUpdate) будет выполнен OnEnvironmentChanged в PhysicsBodyController и тело изменится. Так же изменится PhysicsBodyController.EnvironmentData.

