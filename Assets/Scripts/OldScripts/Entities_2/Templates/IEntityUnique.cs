using System;

namespace Entities_2
{
    public interface IEntityUnique
    {
        string EntityId { get; }
        static string Id { get; }
    }
}
