using System;
using BuildingBlocks.IdsGenerator;
using Flight.Flights.Dtos;
using Flight.Flights.Models;
using MediatR;

namespace Flight.Flights.Features.CreateFlight;

public record CreateFlightCommand(string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, FlightStatus Status, decimal Price) : IRequest<FlightResponseDto>
{
    public long Id { get; set; } = SnowFlakIdGenerator.NewId();
}
