using Woo.Core.Event;
using Woo.Core.Model;

namespace Woo.EventStoreDB.Events
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