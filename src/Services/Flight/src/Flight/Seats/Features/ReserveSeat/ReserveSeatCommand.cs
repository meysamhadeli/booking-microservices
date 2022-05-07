using Flight.Seats.Dtos;
using MediatR;

namespace Flight.Seats.Features.ReserveSeat;

public record ReserveSeatCommand(long FlightId, string SeatNumber) : IRequest<SeatResponseDto>;
