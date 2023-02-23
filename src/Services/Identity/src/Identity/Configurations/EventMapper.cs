namespace Identity.Configurations;

using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }

    public IInternalCommand MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }
}
