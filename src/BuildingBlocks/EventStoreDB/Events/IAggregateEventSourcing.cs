using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;

namespace BuildingBlocks.EventStoreDB.Events
{
    public interface IAggregateEventSourcing : IProjection, IEntity
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IEvent[] ClearDomainEvents();
        long Version { get; }
    }

    public interface IAggregateEventSourcing<out T> : IAggregateEventSourcing
    {
        T Id { get; }
    }
}


