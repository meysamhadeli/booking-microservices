using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core.Model;

public interface IAggregate : IAudit
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IEvent[] ClearDomainEvents();
    long Version { get; set; }
}

public interface IAggregate<out T> : IAggregate
{
    T Id { get; }
}
