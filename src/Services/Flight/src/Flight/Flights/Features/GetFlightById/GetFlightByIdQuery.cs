using BuildingBlocks.Core.CQRS;
using Flight.Flights.Dtos;

namespace Flight.Flights.Features.GetFlightById;

public record GetFlightByIdQuery(long Id) : IQuery<FlightResponseDto>;
