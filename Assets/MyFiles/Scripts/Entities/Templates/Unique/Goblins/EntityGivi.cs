namespace Entities
{
    public class EntityGivi : EntityGoblin, IEntityUnique
    {
        public string EntityId => "Goblin_Givi";

        protected override void Build(EntityBuilder builder)
        {
            base.Build(builder);
            // Уникальные компоненты для Гиви
        }

        protected override void PostBuild(IEntity entity)
        {
            base.PostBuild(entity);
            var info = entity.EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Гиви Хитрый";
            }
        }
    }
}
