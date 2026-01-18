using UnityEngine;

namespace Entities_1
{
    [System.Serializable]
    public abstract class BaseEntity : MonoBehaviour
    {
        public virtual string EntityName { get; set; }
        public virtual string EntityID { get; set; }
        public virtual string EntityPrefabPath { get; set; }
        // public virtual Stats.BaseStats Stats { get; set; }
        public virtual Stats.BaseStats InitStats { get; set; } = new Stats.BaseStats();
        public virtual Stats.BaseStats BaseStats { get; set; }
        public virtual Stats.NowStats NowStats { get; set; }
        // public virtual Stats.EffectModifyList EffectModifyList { get; set; }

        public virtual string Vectors { get; set; } // тут надо придумать название свойства, здесь будут координаты, повороты и размеры 

        [TextArea(5, 20)] public string SaveJSON;
        [TextArea(5, 20)] public string NowJSON;

        protected float statUpdateTimer = 0f;
        // protected const float StatUpdateInterval = 0.5f; // 2 раза в секунду
        protected const float StatUpdateInterval = 3f;

        protected virtual void Awake()
        {
            // var initStats = new Stats.InitStats();
            // Stats = new Stats.BaseStats(initStats);
            // ещё надо получить EntityPrefabPath

            Load();
            Save();
            NowStats = new(BaseStats);

            // Debug.Log(BaseStats.HealthMax);
            // Debug.Log(NowStats.Health.NowValue);

            // BaseStats.HealthMax = 70;
            // Debug.Log(BaseStats.HealthMax);
            // Debug.Log(NowStats.Health.NowValue);

            // NowStats.Health.ChangeNowValue(50, true);
            // Debug.Log(BaseStats.HealthMax);
            // Debug.Log(NowStats.Health.NowValue);
        }

        public virtual void StartDialogue() { }

        public void Spawn() { }
        public void Save() { UpdateNowJSON(); }
        public void Load() { LoadFromSaveJSON(); }

        public void UpdateNowJSON()
        {
            NowJSON = JsonUtility.ToJson(BaseStats, true);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void LoadFromSaveJSON()
        {
            if (!string.IsNullOrEmpty(SaveJSON))
            {
                BaseStats = JsonUtility.FromJson<Stats.BaseStats>(SaveJSON);
            }
            else
            {
                BaseStats = InitStats;
            }
        }

        public void FixedUpdate()
        {
            statUpdateTimer += Time.deltaTime;

            if (statUpdateTimer >= StatUpdateInterval)
            {
                NowStats.Tick(statUpdateTimer); // передаём накопленное время
                statUpdateTimer = 0f;

                // Stats.FloatStat health = NowStats.Health;
                // Debug.Log($"Сейчас здоровья: {health.NowValue}");
                Save();
            }
        }

        public void ReceiveDamage(float amount)
        {
            NowStats.Health.ChangeNowValue(-amount);
            Debug.Log($"{EntityName} получил {amount} урона. Текущее здоровье: {NowStats.Health.NowValue}");
        }

        // protected virtual Stats.InitStats GetDefaultStats()
        // {
        //     return new Stats.InitStats();
        // }

        //         public void UpdateNowJSON()
        //         {
        //             NowJSON = JsonUtility.ToJson(Stats, true);
        // #if UNITY_EDITOR
        //             UnityEditor.EditorUtility.SetDirty(this);
        // #endif
        //         }

        //         public void LoadFromSaveJSON()
        //         {
        //             if (!string.IsNullOrEmpty(SaveJSON))
        //             {
        //                 Stats = JsonUtility.FromJson<Stats.BaseStats>(SaveJSON);
        //             }
        //             else
        //             {
        //                 Stats = new Stats.BaseStats();
        //             }
        //         }
    }
}

// public ResistanceBlock Resistances;

// public List<Effect> ActiveEffects = new();
// public List<Buff> ActiveBuffs = new();

// public void ApplyEffect(Effect effect) { /* Проверка устойчивости и добавление */ }
// public void ApplyBuff(Buff buff) { /* Проверка иммунитетов и т. д. */ }
// public void Tick(float deltaTime) { /* Апдейт баффов и эффектов */ }