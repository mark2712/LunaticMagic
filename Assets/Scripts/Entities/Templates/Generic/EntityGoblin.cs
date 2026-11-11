namespace Entities
{
    public class EntityGoblin : EntityMonster
    {
        public EntityGoblin(EntityData data) : base(data) { }

        protected override void Build()
        {
            base.Build();
            // Специфичные компоненты гоблина, например:
            // EntityRuntime.AddComponent(new GoblinLootComponent());
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            var info = EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Гоблин";
                info.EntityType = "Goblin";
            }
        }
    }
}
