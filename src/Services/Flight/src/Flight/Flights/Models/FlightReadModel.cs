namespace Flight.Flights.Models;

using System;

public class FlightReadModel
{
    public required Guid Id { get; init; }
    public required Guid FlightId { get; init; }
    public required string FlightNumber { get; init; }
    public required Guid AircraftId { get; init; }
    public required DateTime DepartureDate { get; init; }
    public required Guid DepartureAirportId { get; init; }
    public required DateTime ArriveDate { get; init; }
    public required Guid ArriveAirportId { get; init; }
    public required decimal DurationMinutes { get; init; }
    public required DateTime FlightDate { get; init; }
    public required Enums.FlightStatus Status { get; init; }
    public required decimal Price { get; init; }
    public required bool IsDeleted { get; init; }
}
