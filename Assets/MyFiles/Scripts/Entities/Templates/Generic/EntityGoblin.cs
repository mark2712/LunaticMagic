namespace Entities
{
    public class EntityGoblin : EntityMonster
    {
        protected override void Build(EntityBuilder builder)
        {
            base.Build(builder);
            // Специфичные компоненты гоблина, например:
            // builder.AddComponent<GoblinLootComponent>();
        }

        protected override void PostBuild(IEntity entity)
        {
            base.PostBuild(entity);
            var info = entity.EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Гоблин";
                info.EntityType = "Goblin";
            }
        }
    }
}
