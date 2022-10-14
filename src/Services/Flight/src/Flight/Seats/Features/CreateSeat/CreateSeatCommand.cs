using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Dtos;
using Flight.Seats.Models;

namespace Flight.Seats.Features.CreateSeat;

public record CreateSeatCommand(string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId) : ICommand<SeatResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
