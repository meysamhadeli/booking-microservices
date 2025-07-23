using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;

namespace BuildingBlocks.EventStoreDB.Events
{
    public interface IAggregateEventSourcing : IProjection, IEntity
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IDomainEvent[] ClearDomainEvents();
    }

    public interface IAggregateEventSourcing<T> : IAggregateEventSourcing, IEntity<T>
    {
    }
}