namespace Entities
{
    public abstract class EntityNPC : EntityTemplateBase
    {
        protected override void Build(EntityBuilder builder)
        {
            base.Build(builder);
            // Дополнительные компоненты NPC
        }

        protected override void PostBuild(IEntity entity)
        {
            base.PostBuild(entity);
            var info = entity.EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Незнакомец";
                info.EntityType = "Human";
            }
        }
    }
}
