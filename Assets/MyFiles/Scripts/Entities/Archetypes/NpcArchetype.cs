namespace Entities
{
    public class NpcArchetype : IEntityArchetype
    {
        public string Id => "npc";

        public void Build(EntityBuilder builder)
        {
            builder
                .AddComponent<EntityInfoComponent>();
            // .AddComponent<TransformComponent>()
            // .AddComponent<DialogueComponent>()
            // .AddComponent<NpcAIComponent>();
        }
    }
}