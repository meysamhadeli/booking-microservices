using BuildingBlocks.Core.CQRS;
using Flight.Seats.Dtos;

namespace Flight.Seats.Features.ReserveSeat;

public record ReserveSeatCommand(long FlightId, string SeatNumber) : ICommand<SeatResponseDto>, IInternalCommand;
