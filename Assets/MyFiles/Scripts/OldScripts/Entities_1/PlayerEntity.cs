using UnityEngine;

namespace Entities_1
{
    public class PlayerEntity : BaseEntity
    {
        public override string EntityName => "PlayerEntity";

        // protected override void Awake()
        // {
        //     // var initStats = new Stats.InitStats();
        //     // Stats = new Stats.BaseStats(initStats);
        //     base.Awake();

        //     // FloatStat health = Stats.Health;

        //     // // health.OnStateChanged += (cur, old, max) => Debug.Log($"Изменение крайних значений шкалы здоровья: {cur} / {old} / {max}");
        //     // health.OnKnockout += (cur, old, max) => Debug.Log($"Изменение крайних значений шкалы здоровья: {cur} / {old} / {max}");
        // }
    }
}

