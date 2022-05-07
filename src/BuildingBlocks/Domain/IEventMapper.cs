using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Domain;

public interface IEventMapper
{
    IIntegrationEvent Map(IDomainEvent @event);
    IEnumerable<IIntegrationEvent> MapAll(IEnumerable<IDomainEvent> events);
}
