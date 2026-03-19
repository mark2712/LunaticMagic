namespace Entities
{
    public abstract class EntityTemplateBase
    {
        public virtual IEntity Create()
        {
            var builder = new EntityBuilder();
            Build(builder);

            var entity = builder.Build();
            PostBuild(entity);

            return entity;
        }

        protected virtual void Build(EntityBuilder builder)
        {
            if (this is IEntityUnique uniqueTemplate)
            {
                builder.WithId(uniqueTemplate.EntityId);
            }
            builder.AddComponent<EntityInfoComponent>();
        }

        protected virtual void PostBuild(IEntity entity)
        {
            // Базовая реализация может быть пустой
        }
    }
}
