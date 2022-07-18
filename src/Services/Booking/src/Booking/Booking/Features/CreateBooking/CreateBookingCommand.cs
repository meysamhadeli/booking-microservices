using BuildingBlocks.Core.CQRS;
using BuildingBlocks.IdsGenerator;

namespace Booking.Booking.Features.CreateBooking;

public record CreateBookingCommand(long PassengerId, long FlightId, string Description) : ICommand<ulong>
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}

