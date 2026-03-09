using System;
using System.Collections.Generic;

namespace Entities
{
    public interface IEntityArchetype
    {
        string Id { get; }
        void Build(EntityBuilder builder);
    }

    public static class EntityArchetypes
    {
        private static readonly Dictionary<string, IEntityArchetype> _archetypes = new();

        static EntityArchetypes()
        {
            Register(new NpcArchetype());
            // Register(new ChestArchetype());
        }

        public static void Register(IEntityArchetype archetype)
        {
            _archetypes[archetype.Id] = archetype;
        }

        public static IEntity Create(string archetypeId)
        {
            var builder = new EntityBuilder();
            _archetypes[archetypeId].Build(builder);
            return builder.Build();
        }
    }
}