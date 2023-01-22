using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core.Model;

public interface IAggregate : IAudit, IVersion
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IEvent[] ClearDomainEvents();
}

public interface IAggregate<out T> : IAggregate
{
    T Id { get; }
}

public interface IVersion
{
    long Version { get; set; }
}
