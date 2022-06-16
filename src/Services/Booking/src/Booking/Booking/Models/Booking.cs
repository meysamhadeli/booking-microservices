using Booking.Booking.Events.Domain;
using Booking.Booking.Models.ValueObjects;
using BuildingBlocks.EventStoreDB.Events;

namespace Booking.Booking.Models;

public class Booking : AggregateEventSourcing<long>
{
    public Booking()
    {
    }

    public Trip Trip { get; private set; }
    public PassengerInfo PassengerInfo { get; private set; }

    public static Booking Create(long id, PassengerInfo passengerInfo, Trip trip, bool isDeleted = false)
    {
        var booking = new Booking()
        {
            Id = id,
            Trip = trip,
            PassengerInfo = passengerInfo,
            IsDeleted = isDeleted
        };

        var @event = new BookingCreatedDomainEvent(booking.Id, booking.PassengerInfo, booking.Trip, booking.IsDeleted);

        booking.AddDomainEvent(@event);
        booking.Apply(@event);

        return booking;
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case BookingCreatedDomainEvent reservationCreated:
            {
                Apply(reservationCreated);
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
