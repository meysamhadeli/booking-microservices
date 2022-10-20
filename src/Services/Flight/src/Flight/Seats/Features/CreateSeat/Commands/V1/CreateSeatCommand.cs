using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Dtos;

namespace Flight.Seats.Features.CreateSeat.Commands.V1;

public record CreateSeatCommand(string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId) : ICommand<SeatResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
