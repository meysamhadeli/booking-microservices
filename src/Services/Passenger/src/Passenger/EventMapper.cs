using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;

namespace Passenger;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }

    public InternalCommand MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }
}
