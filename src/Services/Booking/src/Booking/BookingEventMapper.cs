using Griffin.Core.EventBus.Messages;
using Griffin.Core;
using Griffin.Core.Event;

namespace Booking;

using Booking.Features.CreatingBook.V1;

public sealed class BookingEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            BookingCreatedDomainEvent e => new BookingCreated(e.Id),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }
}