using BuildingBlocks.Core.CQRS;
using Flight.Flights.Dtos;

namespace Flight.Flights.Features.GetFlightById.Queries.V1;

public record GetFlightByIdQuery(long Id) : IQuery<FlightResponseDto>;
