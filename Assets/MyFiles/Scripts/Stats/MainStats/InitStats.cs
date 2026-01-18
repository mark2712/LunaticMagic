using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class InitStats
    {
        public LVL LVL = new(1, 0);
        public FloatStat Health = new(100f, 1f, 90f);
        public FloatStat Mana = new(100f, 1f, 50f);
        public FloatStat Stamina = new(100f, 1f, 50f);

        public float Stealth = 1f;
        public float Luck = 1f;
        public float Lockpicking = 1f;
        public float Charisma = 1f;

        public float Speed = 1f;
        public Vector2 Look = Vector2.zero;
        public Vector2 Move = Vector2.zero;
    }
}