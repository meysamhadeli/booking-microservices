using BuildingBlocks.Core.CQRS;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Dtos;
using Flight.Seats.Models;

namespace Flight.Seats.Features.CreateSeat;

public record CreateSeatCommand(string SeatNumber, SeatType Type, SeatClass Class, long FlightId) : ICommand<SeatResponseDto>, IInternalCommand
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();
}
