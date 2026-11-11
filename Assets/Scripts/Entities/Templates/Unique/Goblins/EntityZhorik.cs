namespace Entities
{
    [UniqueEntity]
    public class EntityZhorik : EntityGoblin
    {
        public static string Id => "Goblin_Zhorik";

        public EntityZhorik() : base(new EntityData { EntityId = Id }) { }
        public EntityZhorik(EntityData data) : base(data) { }

        protected override void Build()
        {
            base.Build();
            // Уникальные компоненты для Жорика Хмурого
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            var info = EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Жорик Хмурый";
            }
        }
    }
}
