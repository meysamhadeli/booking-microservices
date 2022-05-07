using System.Collections.Generic;
using Flight.Seats.Dtos;
using MediatR;

namespace Flight.Seats.Features.GetAvailableSeats;

public record GetAvailableSeatsQuery(long FlightId) : IRequest<IEnumerable<SeatResponseDto>>;
