namespace Entities
{
    public abstract class EntityMonster : EntityNPC
    {
        protected EntityMonster(EntityData data) : base(data) { }

        protected override void Build()
        {
            base.Build();
            // Дополнительные компоненты монстра, например:
            // EntityRuntime.AddComponent(new EntityStats());
            // EntityRuntime.AddComponent(new MonsterAIComponent());
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            var info = EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Монстр";
                info.EntityType = "Monster";
            }
        }
    }
}
