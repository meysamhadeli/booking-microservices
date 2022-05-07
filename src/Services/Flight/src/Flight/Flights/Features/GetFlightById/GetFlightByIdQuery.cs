using Flight.Flights.Dtos;
using MediatR;

namespace Flight.Flights.Features.GetFlightById;

public record GetFlightByIdQuery(long Id) : IRequest<FlightResponseDto>;
