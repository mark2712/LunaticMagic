using UnityEngine;

namespace Stats
{
    interface IStats
    {
        public LVL LVL { get; set; }

        public FloatStat Health { get; set; }
        public FloatStat Mana { get; set; }
        public FloatStat Stamina { get; set; }

        public float Stealth { get; set; } // скрытность (влияет скорость обнаружения)
        public float Luck { get; set; } // удача (влияет на случайные события)
        public float Lockpicking { get; set; } // навык взлома
        public float Charisma { get; set; } // Обаяние (влияет на цены у торговцев, а так же на скорость изменения отношений)

        public float Speed { get; set; }
        public Vector2 Look { get; set; }
        public Vector2 Move { get; set; }
    }
}

// public int Level { get; set; }
// public int Experience { get; set; }

// public float MaxHealth { get; set; }
// public float MaxMana { get; set; }
// public float MaxStamina { get; set; }

// public float HealthRegen { get; set; }
// public float ManaRegen { get; set; }
// public float StaminaRegen { get; set; }

// public float NowHealth { get; set; }
// public float NowMana { get; set; }
// public float NowStamina { get; set; }