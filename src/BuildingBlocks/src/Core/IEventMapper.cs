using Woo.Core.Event;

namespace Woo.Core;

public interface IEventMapper
{
    IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event);
    IInternalCommand? MapToInternalCommand(IDomainEvent @event);
}