using System;
using System.Collections.Generic;

namespace Story
{
    public class QuestsManager
    {

    }


    public enum QuestStatus
    {
        None,
        Progress,
        Sucsess,
        Fail,
    }

    public interface IQuest
    {
        QuestStatus Status { get; } // текущий статус квеста
        List<QuestStatus> History { get; } // история выполнения квеста (необязательна, но может пригодится чтобы считать когда были неудачные попытки)
        int SucsessCount { get; } // сколько раз квест был выполнен успешно
        int FailCount { get; } // сколько раз квест был выполнен успешно
        void SetStatus(QuestStatus status);
        bool IsConditionAvailable(); // можно ли взять квест
        bool IsConditionComplete(); // проверить условие успешного завершения
        bool IsComplete { get; }
    }

    public abstract class QuestBase : IQuest
    {
        public virtual QuestStatus Status { get; protected set; } = QuestStatus.None;
        public virtual bool IsHistoryOn { get; protected set; } = false;
        public virtual bool IsLoop { get; protected set; } = false;

        public List<QuestStatus> History { get; private set; } = new();
        public int SucsessCount { get; protected set; } = 0;
        public int FailCount { get; protected set; } = 0;

        public bool IsComplete => Status == QuestStatus.Fail || Status == QuestStatus.Sucsess;

        public virtual void SetStatus(QuestStatus status)
        {
            Status = status;

            if (status == QuestStatus.Sucsess)
            {
                SucsessCount++;
            }
            else if (status == QuestStatus.Fail)
            {
                FailCount++;
            }

            if (IsHistoryOn)
            {
                History.Add(Status);
            }

            if (IsLoop && IsComplete)
            {
                Status = QuestStatus.None;
            }
        }

        public virtual bool IsConditionAvailable()
        {
            return Status == QuestStatus.None;
        }

        public abstract bool IsConditionComplete();
    }

    public class QuestEnemyKills : QuestBase
    {
        public override bool IsLoop { get; protected set; } = true;
        public override void SetStatus(QuestStatus status)
        {
            base.SetStatus(status);
            if (SucsessCount >= 10 || FailCount > 1)
            {
                IsLoop = false;
                Status = status;
            }
        }

        public override bool IsConditionComplete()
        {
            return true;
        }
    }


    public class QuestSearchItems : QuestBase
    {
        public IQuest QuestSearchItem = new QuestSearchItem();
        public override bool IsConditionComplete()
        {
            return QuestSearchItem.SucsessCount >= 10;
        }
    }

    public class QuestSearchItem : QuestBase
    {
        public override bool IsLoop => true;
        public override bool IsConditionComplete()
        {
            return true;
        }
    }

    // [Serializable]
    // public class QuestData
    // {
    //     public QuestStatus Status = QuestStatus.None;
}




// 1. Статусы (твой enum, немного расширенный)
// public enum QuestStatus
// {
//     Locked,       // Еще недоступен (например, нужен другой уровень)
//     Available,    // Можно взять у NPC
//     InProgress,   // В процессе
//     Completed,    // Выполнен (награда получена)
//     Failed        // Провален
// }

// // 2. Статичные данные (Definition) - НЕ СОХРАНЯЮТСЯ в сейв
// public class QuestDefinition
// {
//     public string Id; // Уникальный ключ (например, "quest_kill_rats_01")
//     public string Title;
//     public string NextQuestId; // ВАЖНО: Ссылка на следующий квест в цепочке
//     public List<ObjectiveDefinition> Objectives;
// }

// // 3. Сохраняемые данные (State) - ИДУТ В СЕЙВ
// [Serializable]
// public class QuestState
// {
//     public string QuestId;
//     public QuestStatus Status = QuestStatus.Locked;
//     public List<ObjectiveState> ObjectiveStates;
// }

// public abstract class ObjectiveDefinition
// {
//     public string Id;
//     public abstract ObjectiveState CreateState();
// }

// // Пример конкретной цели: Убийство
// public class KillObjectiveDefinition : ObjectiveDefinition
// {
//     public string EnemyId; // Кого убивать
//     public int RequiredAmount; // Сколько

//     public override ObjectiveState CreateState() => new KillObjectiveState(this);
// }

// // Состояние конкретной цели
// [Serializable]
// public class KillObjectiveState : ObjectiveState
// {
//     public int CurrentAmount;

//     // Пустой конструктор для сериализатора
//     public KillObjectiveState() { }

//     public KillObjectiveState(KillObjectiveDefinition def)
//     {
//         Id = def.Id;
//         CurrentAmount = 0;
//         IsCompleted = false;
//     }
// }


// // Глобальное событие
// public struct EnemyKilledEvent
// {
//     public string EnemyId;
// }

// // Логика активного квеста (Runtime)
// public class ActiveQuest : IDisposable
// {
//     private readonly QuestDefinition _definition;
//     private readonly QuestState _state;
//     private CompositeDisposable _disposables = new CompositeDisposable();

//     public ActiveQuest(QuestDefinition definition, QuestState state)
//     {
//         _definition = definition;
//         _state = state;
//     }

//     public void StartListening()
//     {
//         // Подписываемся на глобальное событие смерти врагов
//         MessageBroker.Default.Receive<EnemyKilledEvent>()
//             .Subscribe(OnEnemyKilled)
//             .AddTo(_disposables);
//     }

//     private void OnEnemyKilled(EnemyKilledEvent evt)
//     {
//         // Логика проверки: тот ли враг убит, обновление state.CurrentAmount
//         // Если все Objectives выполнены -> меняем _state.Status на Completed
//         // Вызываем событие завершения квеста
//     }

//     public void Dispose()
//     {
//         _disposables.Dispose(); // Отписываемся, когда квест сдан/провален
//     }
// }

