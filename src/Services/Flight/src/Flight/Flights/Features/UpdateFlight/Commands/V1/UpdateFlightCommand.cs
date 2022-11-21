using System;
using BuildingBlocks.Caching;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Flight.Flights.Dtos;

namespace Flight.Flights.Features.UpdateFlight.Commands.V1;

public record UpdateFlightCommand(long Id, string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, bool IsDeleted, decimal Price) : ICommand<FlightResponseDto>, IInternalCommand, IInvalidateCacheRequest
{
    public string CacheKey => "GetAvailableFlightsQuery";
}
