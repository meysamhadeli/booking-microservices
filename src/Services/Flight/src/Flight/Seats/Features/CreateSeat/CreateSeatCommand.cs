using BuildingBlocks.IdsGenerator;
using Flight.Seats.Dtos;
using Flight.Seats.Models;
using MediatR;

namespace Flight.Seats.Features.CreateSeat;

public record CreateSeatCommand(string SeatNumber, SeatType Type, SeatClass Class, long FlightId) : IRequest<SeatResponseDto>
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();
}
