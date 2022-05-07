using BuildingBlocks.IdsGenerator;
using MediatR;

namespace Booking.Booking.Features.CreateBooking;

public record CreateBookingCommand
    (long PassengerId, long FlightId, string Description) : IRequest<ulong>
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();
}

