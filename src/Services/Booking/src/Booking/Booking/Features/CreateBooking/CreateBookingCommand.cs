using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;

namespace Booking.Booking.Features.CreateBooking;

public record CreateBookingCommand(long PassengerId, long FlightId, string Description) : ICommand<ulong>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}

