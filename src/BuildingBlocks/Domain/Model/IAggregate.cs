using BuildingBlocks.Domain.Event;
using BuildingBlocks.EventStoreDB.Events;

namespace BuildingBlocks.Domain.Model
{
    public interface IAggregate : IProjection, IAuditable
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IEvent[] ClearDomainEvents();
        long Version { get; }
        public bool IsDeleted { get; }
    }

    public interface IAggregate<out T> : IAggregate
    {
        T Id { get; }
    }
}


