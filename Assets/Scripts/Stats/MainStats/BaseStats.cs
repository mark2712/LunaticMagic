using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class BaseStats
    {
        public int Level = 1;
        public int Experience = 0;

        public float HealthNow = 100;
        public float HealthMax = 100;
        public float HealthRegen = 1;

        public float ManaNow = 50;
        public float ManaMax = 50;
        public float ManaRegen = 1;

        public float StaminaNow = 50;
        public float StaminaMax = 50;
        public float StaminaRegen = 1;

        public void OnLevelUp(int level, int experience)
        {
            Level = level;
            Experience = experience;
            HealthMax += HealthMax * 1.2f;
            ManaMax += ManaMax * 1.2f;
            StaminaMax += StaminaMax * 1.1f;
        }
    }
}