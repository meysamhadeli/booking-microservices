using System;
using BuildingBlocks.Caching;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Flight.Flights.Dtos;

namespace Flight.Flights.Features.UpdateFlight.Commands.V1;

public record UpdateFlightCommand : ICommand<FlightResponseDto>, IInvalidateCacheRequest, IInternalCommand
{
    public long Id { get; init; }
    public string FlightNumber { get; init; }
    public long AircraftId { get; init; }
    public long DepartureAirportId { get; init; }
    public DateTime DepartureDate { get; init; }
    public DateTime ArriveDate { get; init; }
    public long ArriveAirportId { get; init; }
    public decimal DurationMinutes { get; init; }
    public DateTime FlightDate { get; init; }

    public Enums.FlightStatus Status { get; init; }

    public bool IsDeleted { get; init; } = false;
    public decimal Price { get; init; }
    public string CacheKey => "GetAvailableFlightsQuery";
}
