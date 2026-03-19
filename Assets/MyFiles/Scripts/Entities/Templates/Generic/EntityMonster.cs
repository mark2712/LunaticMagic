namespace Entities
{
    public abstract class EntityMonster : EntityNPC
    {
        protected override void Build(EntityBuilder builder)
        {
            base.Build(builder);
            // Дополнительные компоненты монстра, например:
            // builder.AddComponent<EntityStats>();
            // builder.AddComponent<MonsterAIComponent>();
        }

        protected override void PostBuild(IEntity entity)
        {
            base.PostBuild(entity);
            var info = entity.EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Монстр";
                info.EntityType = "Monster";
            }
        }
    }
}
