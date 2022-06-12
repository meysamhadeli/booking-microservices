using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Domain.Model;

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IEvent[] ClearDomainEvents();
    long Version { get; set; }
}

public interface IAggregate<out T> : IAggregate
{
    T Id { get; }
}
