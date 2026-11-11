namespace Entities
{
    [UniqueEntity]
    public class EntityMainMenuInitScript : Entity
    {
        public static string Id => "MainMenuInitScript";

        public EntityMainMenuInitScript() : base(new EntityData { EntityId = Id }) { }
        public EntityMainMenuInitScript(EntityData data) : base(data) { }

        protected override void Build()
        {
            base.Build();
            EntityRuntime.AddComponent(new MainMenuInitScriptComponent());
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            var info = EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "MainMenuInitScript";
                info.EntityType = "Script";
            }
        }
    }
}
