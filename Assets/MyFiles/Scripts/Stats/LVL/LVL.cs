using System;
using UnityEngine;

namespace Stats
{
    public class LVL
    {
        public int Level;
        public int Experience;

        public const float baseExp = 100f;
        public const float expFactor = 1.2f;

        public event Action OnLevelUp;

        public LVL(int level = 1, int experience = 0)
        {
            Level = level;
            Experience = experience;
        }

        public void AddExperience(int amount)
        {
            Experience += amount;
            TryLevelUp();
        }

        private void TryLevelUp()
        {
            while (Experience >= GetRequiredExp(Level))
            {
                Experience -= GetRequiredExp(Level);
                Level++;
                Debug.Log($"Level up! New level: {Level}");
                OnLevelUp?.Invoke();
            }
        }

        private int GetRequiredExp(int level)
        {
            return (int)(baseExp * Mathf.Pow(level, expFactor));
        }
    }
}