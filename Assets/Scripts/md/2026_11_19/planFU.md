Я бы выбрал линейный конгруэнтный генератор (LCG) по модулю 150 с динамической сменой параметров раз в 3 секунды. Если разброс таймингов играет вам на руку и делает бои живее, а на длинной дистанции важна именно честная средняя регенерация, этот алгоритм даст максимальную хаотичность при нулевых затратах памяти.
Даже если два NPC окажутся в соседних ячейках 0 и 1, в каждом новом трехсекундном цикле расстояние по времени между их отхилом будет совершенно разным. При этом за каждые 150 вызовов FixedUpdate железно сработают все 150 корзин ровно по одному разу.

Сравнение генераторов случайного обхода для 150 слотов

Чтобы выбрать оптимальный баланс между «рваностью» регена и простотой, посмотрим на три способа перемешивания без создания массивов:

Метод обхода	Качество хаоса (разброс соседей 0 и 1)	Риск паттернов	Затраты CPU и Памяти
1. LCG (Теорема Халла-Добелла)	Максимальное (прыжки выглядят полностью случайно)	Минимальный (порядок кардинально меняется каждые 3 сек)	1 умножение + 1 остаток от деления. 0 байт GC
2. Линейный шаг (start + i * step) % 150	Высокое (соседи всегда разнесены на step тиков)	Средний (внутри одного круга ритм шага постоянный)	1 умножение + 1 остаток от деления. 0 байт GC
3. Тасование массива индексов (Фишер-Йейтс)	Абсолютное (истинный рандом всех перестановок)	Нулевой	Массив int[150] в памяти + 150 вызовов Random раз в 3 сек


Математика под число 150 (почему LCG тут идеален)

Формула LCG перечисляет индексы так: currentBucket = (a * currentBucket + c) % 150.
Чтобы формула не зациклилась раньше времени и обошла все 150 ячеек без единого повтора, для числа 150 (делители: 2, 3, 5) нужны особые коэффициенты:
• Множитель a: число a - 1 должно делиться на 30 (2 * 3 * 5). Нам идеально подходят константы: 31, 61, 91, 121.
• Приращение c: любое число, которое не делится на 2, 3 и 5 (взаимно простое с 150).
Меняя a, c и стартовую ячейку каждые 150 тиков на основе текущего времени, мы получаем непредсказуемую последовательность на каждый новый круг.

Готовый скрипт для Unity (C#)

Здесь объединены быстрая структура корзин (чтобы не плодить новые массивы, когда NPC больше 150) и LCG-генератор, который перенастраивается от хэша времени каждые 3 секунды.
csharp
using System.Collections.Generic;
using UnityEngine;

public interface IRegenerable
{
    void OnRegenTick();
    int RegenBucketIndex { get; set; }
}

public class ChaoticRegenManager : MonoBehaviour
{
    private const int TicksCycle = 150; // 3 секунды (при FixedUpdate 50 Гц)

    // Множители 'a', где (a - 1) кратно 30 (кроме 1, чтобы не было простого шага)
    private static readonly int[] ValidMultipliers = { 31, 61, 91, 121 };

    // Приращения 'c', взаимно простые с 150 (не делятся на 2, 3 и 5)
    private static readonly int[] ValidIncrements = 
    { 
        7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 49, 
        53, 59, 61, 67, 71, 73, 77, 79, 83, 89, 91, 97, 101, 
        103, 107, 109, 113, 119, 121, 127, 131, 133, 137, 139, 143, 149 
    };

    private readonly List<IRegenerable>[] _buckets = new List<IRegenerable>[TicksCycle];
    
    private int _ticksProcessedInCycle = 0;
    private int _nextRegisterSlot = 0;

    // Текущие параметры генератора на трехсекундный цикл
    private int _currentBucket;
    private int _a;
    private int _c;

    private void Awake()
    {
        for (int i = 0; i < TicksCycle; i++)
        {
            _buckets[i] = new List<IRegenerable>(8);
        }
        
        SetupNewRandomCycle();
    }

    // Регистрация NPC: теперь можно класть просто по порядку (0, 1, 2...),
    // так как сам обход ячеек полностью хаотичен!
    public void Register(IRegenerable entity)
    {
        int slot = _nextRegisterSlot;
        _nextRegisterSlot = (_nextRegisterSlot + 1) % TicksCycle;

        entity.RegenBucketIndex = slot;
        _buckets[slot].Add(entity);
    }

    public void Unregister(IRegenerable entity)
    {
        int slot = entity.RegenBucketIndex;
        if (slot < 0 || slot >= TicksCycle) return;

        var list = _buckets[slot];
        int idx = list.IndexOf(entity);
        if (idx != -1)
        {
            int last = list.Count - 1;
            list[idx] = list[last];
            list.RemoveAt(last);
        }

        entity.RegenBucketIndex = -1;
    }

    private void FixedUpdate()
    {
        // 1. Выполняем всех NPC в текущей случайной ячейке
        var list = _buckets[_currentBucket];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            list[i].OnRegenTick();
        }

        // 2. Вычисляем следующую псевдослучайную ячейку за O(1)
        _currentBucket = (_a * _currentBucket + _c) % TicksCycle;

        // 3. Считаем тики; когда прошли все 150 — генерируем новый порядок из времени
        _ticksProcessedInCycle++;
        if (_ticksProcessedInCycle >= TicksCycle)
        {
            _ticksProcessedInCycle = 0;
            SetupNewRandomCycle();
        }
    }

    private void SetupNewRandomCycle()
    {
        // Берем биты текущего времени и перемешиваем (быстрый целочисленный хэш),
        // чтобы соседние секунды давали совершенно разные числа
        uint timeSeed = (uint)(Time.realtimeSinceStartupAsDouble * 100000.0);
        timeSeed ^= timeSeed >> 16;
        timeSeed *= 0x85ebca6b;
        timeSeed ^= timeSeed >> 13;

        _a = ValidMultipliers[timeSeed % (uint)ValidMultipliers.Length];
        _c = ValidIncrements[(timeSeed >> 4) % (uint)ValidIncrements.Length];
        _currentBucket = (int)((timeSeed >> 8) % TicksCycle);
    }
}
Используйте код с осторожностью.

Приятный бонус такого подхода

Обратите внимание на метод Register: за счет того, что чтение массива стало псевдослучайным, вам больше не нужно высчитывать сложную схему вставки (0, 75, 37, 112). Вы кладете NPC подряд в 0, 1, 2, 3..., что гарантирует идеально ровное количество сущностей в каждом списке (никаких перекосов нагрузки на процессор по кадрам), а визуальный разброс берет на себя математика в FixedUpdate.
Нюанс, если NPC больше 150: два моба, попавшие в одну и ту же корзину (например, 1-й и 151-й), всё равно будут обновляться в один тик. Если и их нужно развести, в методе SetupNewRandomCycle или прямо при спавне можно случайно менять местами пары элементов между разными корзинами, но обычно при 150 слотах совпадение раз в 150 мобов глазу совершенно незаметно.
Чтобы убедиться, что система идеально ляжет в ваш проект, подскажите:
• Нужна ли синхронизация регенерации по сети (мультиплеер, где у всех клиентов должен быть одинаковый seed вместо локального времени)?
• Планируются ли сущности с разной скоростью тика (например, боссы регенят раз в 1 секунду, а обычные мобы — раз в 3)?

--- 

Вообще я преследовал 2 цели:
1) чтобы регенерация у нескольких разных сущностей происходила не одновременно
2) оптимизировать нагрузку

То что теперь регенерация рандомизирована это просто приятный бонус. 

Однако тут действительно есть проблема: если я хочу рандомизировать ещё и реген маны и выносливости то алгоритм не подойдет. Ия никак не могу додуматься что мне делать. С одной стороны хочется сделать методы которые просто будут выполнятся раз в 1, 3, 5 10 сек. С другой стороны мне это ничего не даст, все будет выполнятся в FixedUpdate на 150 и тд кадре что даст нагрузк на этот кадр. И у всех все шкалы изменятся одновременно.

--- 

Я бы выбрал паттерн Колеса Времени (Hashed Timing Wheel) на базе единого кольцевого массива задач, где в ячейках хранятся не сами NPC, а отдельные задачи (тик здоровья, тик маны, тик выносливости). Я взял на заметку обе ваши главные цели — убрать микрофризы за счет ровной нагрузки по кадрам и разнести регенерацию здоровья, маны и выносливости во времени, чтобы шкалы у разных сущностей обновлялись вразнобой, а не синхронно.
Ваша догадка абсолютно верна: если вызывать методы просто каждые 50, 150 и 250 кадров (tick % 150 == 0), то на 150-м кадре совпадут 1-секундные и 3-секундные таймеры, а на 1500-м кадре (30-я секунда) одновременно бахнут вообще все таймеры (1, 3, 5 и 10 секунд) у всех NPC в игре. Это даст жесткий спайк процессора и синхронное дергание всех полосок UI.

Как разорвать синхронность разных таймеров (Фазовый сдвиг + Джиттер)

Секрет в том, чтобы отказаться от общих счетчиков для всей игры и планировать следующий кадр срабатывания индивидуально для каждой задачи.
Допустим, самый долгий ваш таймер — 10 секунд (500 тиков FixedUpdate). Мы создаем массив-колесо на 512 корзин (степень двойки работает быстрее всего за счет побитового AND вместо деления %). Указатель _currentTick просто идет по кругу.
Когда нужно запланировать реген с базовым интервалом N тиков (например, 150 для здоровья, 50 для выносливости, 250 для маны):
1. При спавне (Фазовый сдвиг): Мы кидаем первую задачу не на текущий_тик + N, а на текущий_тик + Random(0, N). Уже на этом этапе здоровье, мана и стамина даже у одного и того же NPC разъезжаются по разным кадрам!
2. При срабатывании (Мягкий рандом / Джиттер): Когда колесо доходит до нужной корзины и выполняет метод, задача не остается лежать на месте — она перекладывает саму себя вперед на N + Random(-разброс, +разброс) тиков. Например, для 3-секундного регена (150 тиков) шаг будет случайным от 135 до 165 тиков (2.7 – 3.3 сек). В среднем на дистанции это строгие 3 секунды, но пики нагрузки размываются в пыль, а шкалы живут своей естественной жизнью.

Сравнение архитектур для мульти-таймеров (1, 3, 5, 10 сек)


Подход	Распределение нагрузки на CPU	Независимость ХП / Маны / Стамины	Поддержка любых интервалов (1, 3, 5, 10с)	Память и GC в FixedUpdate
1. Колесо времени (Timing Wheel) с переброской задач	Идеальное (равномерный белый шум)	Полная (каждая шкала живет в своем ритме)	Любые до размера колеса (и даже больше)	0 байт GC (массив структур без аллокаций)
2. Отдельный массив на каждый интервал (50, 150, 250, 500)	Хорошее, но в 4 раза больше проверок за тик	Высокая	Жестко привязана к созданным массивам	0 байт GC, но много дублирования кода
3. Очередь с приоритетом (Min-Heap по времени)	Хорошее	Полная	Абсолютно любые (хоть часы)	Сложнее удаление при смерти NPC (O(log N))


Готовая высокопроизводительная реализация для Unity (Zero-GC)

Чтобы не плодить тысячи объектов в памяти (ведь теперь у 200 NPC с 3 шкалами будет 600 отдельных таймеров), мы сложим все задачи в плоский массив структур (struct), связав их в быстрые односвязные/двусвязные списки по индексам. Никаких List<> на каждую ячейку, никаких вызовов Action с аллокациями памяти — чистая работа с индексами в кэше процессора.

1. Менеджер периодических задач (PeriodicTickScheduler.cs)

csharp
using System;
using UnityEngine;

// Интерфейс для ваших NPC (или отдельных компонентов здоровья/маны)
public interface ITickReceiver
{
    // timerType позволяет в одном NPC различать, что именно тикнуло (0 - ХП, 1 - Мана, 2 - Стамина)
    void OnScheduledTick(byte timerType);
}

public class PeriodicTickScheduler : MonoBehaviour
{
    // 1024 тика при 50 Гц = запас до ~20.4 секунд на любой таймер.
    // Обязательно степень двойки (512, 1024, 2048) для мгновенного взятия индекса через '& Mask'
    private const int WheelSize = 1024;
    private const int WheelMask = WheelSize - 1;

    private struct ScheduledTask
    {
        public ITickReceiver Receiver;
        public ushort BaseIntervalTicks; // Например: 50 (1с), 150 (3с), 250 (5с), 500 (10с)
        public ushort JitterTicks;       // Разброс +/- в тиках (например, 10 тиков = +/- 0.2 сек)
        public byte TimerType;           // Тип шкалы (ХП, Мана, Выносливость)
        
        // Связи внутри корзины для добавления и удаления за O(1) без List<>
        public int BucketIndex;
        public int PrevTask;
        public int NextTask;
        public bool IsActive;
    }

    // В массиве корзин хранится только индекс первой задачи (-1, если корзина пуста)
    private readonly int[] _bucketHeads = new int[WheelSize];

    // Пул всех задач (автоматически расширится, если NPC станет больше)
    private ScheduledTask[] _tasks = new ScheduledTask[512];
    private int[] _freeTaskIds = new int[512];
    private int _freeCount = 0;
    private int _maxUsedId = 0;

    private int _currentTick = 0;
    private uint _rngState = 123456789; // Свой сверхбыстрый рандом Xorshift

    private void Awake()
    {
        Array.Fill(_bucketHeads, -1);
        uint seed = (uint)(Time.realtimeSinceStartupAsDouble * 100000.0) | 1u;
        _rngState = seed;
    }

    /// <summary>
    /// Регистрирует периодический метод и возвращает ID таймера (сохраните его для удаления при смерти NPC).
    /// </summary>
    /// <param name="intervalSeconds">Интервал в секундах (1, 3, 5, 10 — до 18 сек при WheelSize=1024)</param>
    /// <param name="jitterPercent">Доля рандома от 0f до 0.3f (например, 0.1f даст разброс +/- 10%)</param>
    public int Schedule(ITickReceiver receiver, byte timerType, float intervalSeconds, float jitterPercent = 0.1f)
    {
        int baseTicks = Mathf.Max(1, Mathf.RoundToInt(intervalSeconds / Time.fixedDeltaTime));
        int jitterTicks = Mathf.RoundToInt(baseTicks * Mathf.Clamp(jitterPercent, 0f, 0.45f));

        if (baseTicks + jitterTicks >= WheelSize)
        {
            Debug.LogWarning($"Интервал {intervalSeconds}с слишком велик для WheelSize={WheelSize}. Увеличьте WheelSize!");
            baseTicks = WheelSize - jitterTicks - 1;
        }

        int id = AllocateTaskSlot();
        _tasks[id] = new ScheduledTask
        {
            Receiver = receiver,
            BaseIntervalTicks = (ushort)baseTicks,
            JitterTicks = (ushort)jitterTicks,
            TimerType = timerType,
            IsActive = true,
            BucketIndex = -1,
            PrevTask = -1,
            NextTask = -1
        };

        // ГЛАВНАЯ МАГИЯ №1: Первый запуск раскидываем случайно от 1 до baseTicks!
        // Даже если создать 500 NPC в одном кадре, их ХП, мана и стамина лягут в разные корзины.
        int initialDelay = FastRandomRange(1, baseTicks + 1);
        int targetBucket = (_currentTick + initialDelay) & WheelMask;
        
        InsertIntoBucket(id, targetBucket);
        return id;
    }

    /// <summary>
    /// Мгновенное удаление таймера за O(1) (вызывать при смерти/выключении NPC).
    /// </summary>
    public void Cancel(int taskId)
    {
        if (taskId < 0 || taskId >= _maxUsedId || !_tasks[taskId].IsActive) return;

        RemoveFromBucket(taskId);
        _tasks[taskId].IsActive = false;
        _tasks[taskId].Receiver = null; // Освобождаем ссылку для GC

        if (_freeCount == _freeTaskIds.Length)
            Array.Resize(ref _freeTaskIds, _freeTaskIds.Length * 2);
            
        _freeTaskIds[_freeCount++] = taskId;
    }

    private void FixedUpdate()
    {
        // Забираем всю цепочку задач текущего тика и сразу очищаем голову корзины
        int taskId = _bucketHeads[_currentTick];
        _bucketHeads[_currentTick] = -1;

        while (taskId != -1)
        {
            // Сохраняем следующую задачу до того, как текущая переедет в новую корзину
            int nextId = _tasks[taskId].NextTask;
            
            _tasks[taskId].PrevTask = -1;
            _tasks[taskId].NextTask = -1;
            _tasks[taskId].BucketIndex = -1;

            // 1. Вызываем полезную логику у сущности
            var receiver = _tasks[taskId].Receiver;
            byte type = _tasks[taskId].TimerType;
            receiver.OnScheduledTick(type);

            // 2. Проверяем, не отменил ли сам NPC этот таймер внутри OnScheduledTick (например, умер от яда или полностью отхилился)
            if (_tasks[taskId].IsActive)
            {
                // ГЛАВНАЯ МАГИЯ №2: Перекидываем задачу вперед со случайным микро-смещением
                int jitter = _tasks[taskId].JitterTicks > 0 
                    ? FastRandomRange(-_tasks[taskId].JitterTicks, _tasks[taskId].JitterTicks + 1) 
                    : 0;
                    
                int nextDelay = Mathf.Max(1, _tasks[taskId].BaseIntervalTicks + jitter);
                int nextBucket = (_currentTick + nextDelay) & WheelMask;
                
                InsertIntoBucket(taskId, nextBucket);
            }

            taskId = nextId;
        }

        // Двигаем стрелку колеса на 1 шаг вперед
        _currentTick = (_currentTick + 1) & WheelMask;
    }

    // --- Вспомогательные методы управления памятью и списками за O(1) ---

    private int AllocateTaskSlot()
    {
        if (_freeCount > 0)
            return _freeTaskIds[--_freeCount];

        if (_maxUsedId == _tasks.Length)
            Array.Resize(ref _tasks, _tasks.Length * 2);

        return _maxUsedId++;
    }

    private void InsertIntoBucket(int taskId, int bucket)
    {
        int oldHead = _bucketHeads[bucket];
        _tasks[taskId].BucketIndex = bucket;
        _tasks[taskId].PrevTask = -1;
        _tasks[taskId].NextTask = oldHead;

        if (oldHead != -1)
            _tasks[oldHead].PrevTask = taskId;

        _bucketHeads[bucket] = taskId;
    }

    private void RemoveFromBucket(int taskId)
    {
        int bucket = _tasks[taskId].BucketIndex;
        if (bucket == -1) return; // Задача прямо сейчас выполняется в FixedUpdate

        int prev = _tasks[taskId].PrevTask;
        int next = _tasks[taskId].NextTask;

        if (prev != -1) _tasks[prev].NextTask = next;
        else _bucketHeads[bucket] = next;

        if (next != -1) _tasks[next].PrevTask = prev;

        _tasks[taskId].BucketIndex = -1;
    }

    // Сверхбыстрый генератор псевдослучайных чисел Xorshift32 (быстрее UnityEngine.Random в разы)
    private int FastRandomRange(int minInclusive, int maxExclusive)
    {
        _rngState ^= _rngState << 13;
        _rngState ^= _rngState >> 17;
        _rngState ^= _rngState << 5;
        uint range = (uint)(maxExclusive - minInclusive);
        return minInclusive + (int)(_rngState % range);
    }
}
Используйте код с осторожностью.

2. Как этим пользоваться внутри NPC (NpcStats.cs)

В самом скрипте NPC вообще не нужен Update или FixedUpdate! Вы просто регистрируете нужные шкалы с любыми секундами при активации объекта:
csharp
using UnityEngine;

public class NpcStats : MonoBehaviour, ITickReceiver
{
    private const byte StatHealth = 0;
    private const byte StatMana = 1;
    private const byte StatStamina = 2;
    private const byte AiCheck = 3;

    [SerializeField] private PeriodicTickScheduler scheduler;

    private int _hpTimerId = -1;
    private int _manaTimerId = -1;
    private int _staminaTimerId = -1;
    private int _aiTimerId = -1;

    private void OnEnable()
    {
        // Выносливость — раз в 1 сек (+/- 10%)
        _staminaTimerId = scheduler.Schedule(this, StatStamina, intervalSeconds: 1f, jitterPercent: 0.1f);
        
        // Здоровье — раз в 3 сек (+/- 15%)
        _hpTimerId = scheduler.Schedule(this, StatHealth, intervalSeconds: 3f, jitterPercent: 0.15f);
        
        // Мана — раз в 5 сек (+/- 10%)
        _manaTimerId = scheduler.Schedule(this, StatMana, intervalSeconds: 5f, jitterPercent: 0.1f);

        // Бонусом сюда же можно вешать редкую логику (например, поиск целей или автосохранение раз в 10 сек!)
        _aiTimerId = scheduler.Schedule(this, AiCheck, intervalSeconds: 10f, jitterPercent: 0.05f);
    }

    private void OnDisable()
    {
        if (scheduler == null) return;
        
        scheduler.Cancel(_staminaTimerId);
        scheduler.Cancel(_hpTimerId);
        scheduler.Cancel(_manaTimerId);
        scheduler.Cancel(_aiTimerId);
    }

    public void OnScheduledTick(byte timerType)
    {
        switch (timerType)
        {
            case StatStamina:
                // +Выносливость
                break;
            case StatHealth:
                // +Здоровье
                break;
            case StatMana:
                // +Мана
                break;
            case AiCheck:
                // Редкая тяжелая проверка раз в 10 секунд
                break;
        }
    }
}
Используйте код с осторожностью.

Почему это закроет вопрос раз и навсегда

1. Универсальность: Теперь вам не нужно думать о математике делителей (как было с 150). Вы можете задавать 1f, 3f, 5f, 10f или хоть 2.35f секунды — всё работает в едином колесе. На этот же планировщик вы сможете повесить яды/кровотечения (тик раз в 0.5 сек), перепроверку дистанции до игрока у далеких мобов (раз в 2 сек) и редкие эффекты окружения (раз в 10 сек).
2. Идеальная линия в профайлере: В пустой корзине FixedUpdate делает ровно одну проверку if (taskId != -1) и один сдвиг индекса. А задачи за счет initialDelay и jitterPercent самобалансируются по всем 1024 слотам как песок в часах — никаких «роковых 150-х кадров».
3. Никакой нагрузки в покое (Совет на вырост): Так как Schedule и Cancel работают мгновенно за O(1), вы можете даже не крутить таймеры у здоровых NPC! Упала выносливость с 100 до 90 — вызвали Schedule. Дотикала обратно до 100 — прямо внутри OnScheduledTick вызвали scheduler.Cancel(_staminaTimerId). Тогда 1000 стоящих в городе NPC с полными статами будут тратить ровно 0% процессора.
