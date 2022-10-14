using System;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.IdsGenerator;
using Flight.Flights.Dtos;
using Flight.Flights.Models;

namespace Flight.Flights.Features.CreateFlight;

public record CreateFlightCommand(string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price) : ICommand<FlightResponseDto>, IInternalCommand
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}
