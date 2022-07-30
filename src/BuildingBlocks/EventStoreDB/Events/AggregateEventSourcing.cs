using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;

namespace BuildingBlocks.EventStoreDB.Events
{
    public abstract record AggregateEventSourcing<TId> : Audit, IAggregateEventSourcing<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public IDomainEvent[] ClearDomainEvents()
        {
            var dequeuedEvents = _domainEvents.ToArray();

            _domainEvents.Clear();

            return dequeuedEvents;
        }

        public virtual void When(object @event) { }

        public long Version { get; protected set; } = -1;

        public TId Id { get; protected set;  }
    }
}

