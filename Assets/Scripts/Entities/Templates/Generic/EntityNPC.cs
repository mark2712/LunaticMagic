namespace Entities
{
    public abstract class EntityNPC : Entity
    {
        protected EntityNPC(EntityData data) : base(data) { }

        protected override void Build()
        {
            base.Build();
            // Дополнительные компоненты NPC
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            var info = EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Незнакомец";
                info.EntityType = "Human";
            }
        }
    }
}
