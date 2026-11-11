using System;

namespace Entities
{
    public interface IEntityUnique
    {
        string EntityId { get; }
        static string Id { get; }
    }
}
