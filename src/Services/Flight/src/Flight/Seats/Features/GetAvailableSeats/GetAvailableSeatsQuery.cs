using System.Collections.Generic;
using BuildingBlocks.Core.CQRS;
using Flight.Seats.Dtos;
using MediatR;

namespace Flight.Seats.Features.GetAvailableSeats;

public record GetAvailableSeatsQuery(long FlightId) : IQuery<IEnumerable<SeatResponseDto>>;
