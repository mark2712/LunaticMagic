namespace Entities
{
    public class EntityMainMenuInitScript : EntityTemplateBase, IEntityUnique
    {
        public string EntityId => "MainMenuInitScript";

        protected override void Build(EntityBuilder builder)
        {
            base.Build(builder);
            builder.AddComponent<MainMenuInitScriptComponent>();
        }

        protected override void PostBuild(IEntity entity)
        {
            base.PostBuild(entity);
            var info = entity.EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "MainMenuInitScript";
                info.EntityType = "Script";
            }
        }
    }
}
