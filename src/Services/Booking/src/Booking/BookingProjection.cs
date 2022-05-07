using Booking.Booking.Events.Domain;
using Booking.Data;
using BuildingBlocks.EventStoreDB.Events;
using BuildingBlocks.EventStoreDB.Projections;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Booking;

public class BookingProjection : IProjectionProcessor
{
    private readonly BookingDbContext _bookingDbContext;

    public BookingProjection(BookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task ProcessEventAsync<T>(StreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : INotification
    {
        switch (streamEvent.Data)
        {
            case BookingCreatedDomainEvent reservationCreatedDomainEvent:
                await Apply(reservationCreatedDomainEvent, cancellationToken);
                break;
        }
    }

    private async Task Apply(BookingCreatedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var reservation =
            await _bookingDbContext.Bookings.SingleOrDefaultAsync(x => x.Id == @event.Id && !x.IsDeleted,
                cancellationToken);

        if (reservation == null)
        {
            var model = Booking.Models.Booking.Create(@event.Id, @event.PassengerInfo, @event.Trip, @event.IsDeleted);

            await _bookingDbContext.Set<Booking.Models.Booking>().AddAsync(model, cancellationToken);
            await _bookingDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
