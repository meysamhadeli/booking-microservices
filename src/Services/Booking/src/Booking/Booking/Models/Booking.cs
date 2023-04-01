using Booking.Booking.Models.ValueObjects;
using BuildingBlocks.EventStoreDB.Events;

namespace Booking.Booking.Models;

using Features.CreatingBook.Commands.V1;

public record Booking : AggregateEventSourcing<Guid>
{
    public Trip Trip { get; private set; }
    public PassengerInfo PassengerInfo { get; private set; }

    public static Booking Create(Guid id, PassengerInfo passengerInfo, Trip trip, bool isDeleted = false, long? userId = null)
    {
        var booking = new Booking { Id = id, Trip = trip, PassengerInfo = passengerInfo, IsDeleted = isDeleted };

        var @event = new BookingCreatedDomainEvent(booking.Id, booking.PassengerInfo, booking.Trip)
        {
            IsDeleted = booking.IsDeleted,
            CreatedAt = DateTime.Now,
            CreatedBy = userId
        };

        booking.AddDomainEvent(@event);
        booking.Apply(@event);

        return booking;
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case BookingCreatedDomainEvent bookingCreated:
            {
                Apply(bookingCreated);
                return;
            }
        }
    }

    private void Apply(BookingCreatedDomainEvent @event)
    {
        Id = @event.Id;
        Trip = @event.Trip;
        PassengerInfo = @event.PassengerInfo;
        IsDeleted = @event.IsDeleted;
        Version++;
    }
}
