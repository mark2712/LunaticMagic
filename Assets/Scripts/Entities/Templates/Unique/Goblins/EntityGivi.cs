namespace Entities
{
    [UniqueEntity]
    public class EntityGivi : EntityGoblin
    {
        public static string Id => "Goblin_Givi";

        public EntityGivi() : base(new EntityData { EntityId = Id }) { }
        public EntityGivi(EntityData data) : base(data) { }

        protected override void Build()
        {
            base.Build();
            // Уникальные компоненты для Гиви
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            var info = EntityRuntime.GetComponent<EntityInfoComponent>();
            if (info != null)
            {
                info.Name = "Гиви Хитрый";
            }
        }
    }
}
