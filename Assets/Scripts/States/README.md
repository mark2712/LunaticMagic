# State Machine (SM) System

## Как пользоваться

### Создание и регистрация SM

1. Создайте класс `SM` (создаются без иерархии):
```csharp
public class HandRightSM : SM
{
    public override State DefaultState => new RHandEmpty();
}
```

2. Зарегистрируйте SM в конструкторе `SMController`:
```csharp
HandRightSM = new();
Register(HandRightSM);
```

3. Создай базовый State для этой SM
```csharp
public abstract class HandRightBase : State
{
    public override SM SM => SMController.HandRightSM;
}
```


#### MainSM является главным

Именно он используется для методов MonoBehaviour в SMController, а так же все события идут от MainSM. `class SM -> MainSM -> OtherSM`

```csharp
public class MainSM : SM
{
    public override State DefaultState => GetGameState();
}
```

## Обработка событий и работа с иерархией SM

Всё в системе состояний следует делать через события, например `public virtual State Mouse1Performed() { return null; }`

Иерархия между SM задается путем предечи событий из одной SM в другую. Например сначала используется возможность нажать Mouse1 в состоянии из MainSM, потом возможность нажать Mouse1 в HandsSM и только потом сработает Hit. Если BaseGame не подрузамевает нажатие Mouse1 (нет метода Mouse1Performed) или в TakeWeapon нет Mouse1Performed то ничего не произойдёт.

```csharp
// BaseGame (MainSM)
public override State Mouse1Performed()
{
    return SMController.HandsSM.State.Mouse1Performed();
}

// TakeWeapon (HandsSM)
public override State Mouse1Performed()
{
    return new Hit();
}
```

Все события идут от MainSM.

Не обязательно создавать `public virtual State OnOtherChanged() { return null; }` в `State`. 

Вместо этого можно использовать `RegisterEvent`.


### ConflictRule

Для избежания конфликтов между разными SM рекомендуется использовать `ConflictRule`. Можно создать несколько разных UIModalSM и настроить ConflictRule между ними.

Можно добавить в ConflictRule родительское состоние и тогда в него попадут все наследники. Так можно настроить ConflictRule для целой SM.

```csharp
public class TakeWeapon : BaseHands
{
    public override List<ConflictRule> Conflicts => new()
    {
        new ConflictAll<Inventory>(),
    };
    // ...
}
```

В этом примере нельзя одновременно открыть инвентарь и держать оружие. При переходе в TakeWeapon инвентарь будет закрыт, тоесть ModalSM перейдёт в `SM.DefaultState`. 

Так же можно перейти и в другое состояние `new ConflictAll<Inventory, NoneModal>()`


## Избегай слоёв 

- **Слои (GoToLayer и т.д.)** - лучше не использовать без крайней необходимости
