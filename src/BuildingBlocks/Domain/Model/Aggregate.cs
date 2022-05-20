using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Domain.Model
{
    public abstract class Aggregate : Aggregate<long>
    {
    }

    public abstract class Aggregate<TId> : Entity, IAggregate<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public IEvent[] ClearDomainEvents()
        {
            IEvent[] dequeuedEvents = _domainEvents.ToArray();

            _domainEvents.Clear();

            return dequeuedEvents;
        }

        public virtual void When(object @event) { }

        public long Version { get; protected set; } = -1;

        public TId Id { get; protected set;  }
    }
}
