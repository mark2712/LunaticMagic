namespace Entities
{
    public class EntityZhorik : EntityGoblin, IEntityUnique
    {
        public string EntityId => "Goblin_Zhorik";

        protected override void Build(EntityBuilder builder)
        {
            base.Build(builder);
            // Уникальные компоненты для Жорика Хмурого
        }

        protected override void PostBuild(IEntity entity)
        {
            base.PostBuild(entity);
            var info = entity.EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Жорик Хмурый";
            }
        }
    }
}
