namespace Entities
{
    [UniqueEntity]
    public class EntityNewGameInitScript : Entity
    {
        public static string Id => "NewGameInitScript";

        public EntityNewGameInitScript() : base(new EntityData { EntityId = Id }) { }
        public EntityNewGameInitScript(EntityData data) : base(data) { }

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
                info.Name = "NewGameInitScript";
                info.EntityType = "Script";
            }
        }
    }
}
