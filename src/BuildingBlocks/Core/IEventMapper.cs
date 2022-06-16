using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core;

public interface IEventMapper
{
    IIntegrationEvent Map(IDomainEvent @event);
    IEnumerable<IIntegrationEvent> MapAll(IEnumerable<IDomainEvent> events);
}
