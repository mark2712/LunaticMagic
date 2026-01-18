namespace Stats
{
    public class NowStats
    {
        public LVL LVL;
        public FloatStat Health;
        public FloatStat Mana;
        public FloatStat Stamina;

        protected readonly BaseStats _baseStats;

        public NowStats(BaseStats baseStats)
        {
            _baseStats = baseStats;

            LVL = new LVL(baseStats.Level, baseStats.Experience);
            LVL.OnLevelUp += () =>
            {
                _baseStats.OnLevelUp(LVL.Level, LVL.Experience);
                RefreshFromBase(_baseStats);
                Health.ChangeNowValue(Health.MaxValue);
                Mana.ChangeNowValue(Mana.MaxValue);
                Stamina.ChangeNowValue(Stamina.MaxValue);
            };

            RefreshFromBase(_baseStats);
        }

        protected void RefreshFromBase(BaseStats baseStats)
        {
            Health = new FloatStat(
                baseStats.HealthMax,
                baseStats.HealthRegen,
                baseStats.HealthNow
            );
            Health.OnNowValueChanged += (cur, old, max) =>
            {
                _baseStats.HealthNow = cur;
            };

            Mana = new FloatStat(
                baseStats.ManaMax,
                baseStats.ManaRegen,
                baseStats.ManaNow
            );
            Mana.OnNowValueChanged += (cur, old, max) =>
            {
                _baseStats.ManaNow = cur;
            };

            Stamina = new FloatStat(
                baseStats.StaminaMax,
                baseStats.StaminaRegen,
                baseStats.StaminaNow
            );
            Stamina.OnNowValueChanged += (cur, old, max) =>
            {
                _baseStats.StaminaNow = cur;
            };
        }

        public void Tick(float deltaTime)
        {
            Health.Regenerate(deltaTime);
            Mana.Regenerate(deltaTime);
            Stamina.Regenerate(deltaTime);
        }
    }
}