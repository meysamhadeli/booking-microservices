using Booking.Booking.Events.Domain;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;

namespace Booking;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            BookingCreatedDomainEvent e => new BookingCreated(e.Id),
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
