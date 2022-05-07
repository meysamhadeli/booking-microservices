using System;
using BuildingBlocks.Caching;
using Flight.Flights.Dtos;
using Flight.Flights.Models;
using MediatR;

namespace Flight.Flights.Features.UpdateFlight;

public record UpdateFlightCommand : IRequest<FlightResponseDto>, IInvalidateCacheRequest
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

    public FlightStatus Status { get; init; }

    public bool IsDeleted { get; init; } = false;
    public decimal Price { get; init; }
    public string CacheKey => "GetAvailableFlightsQuery";
}
