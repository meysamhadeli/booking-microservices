namespace Flight.Flights.Models;

using System;

public class FlightReadModel
{
    public Guid Id { get; init; }
    public Guid FlightId { get; init; }
    public string FlightNumber { get; init; }
    public Guid AircraftId { get; init; }
    public DateTime DepartureDate { get; init; }
    public Guid DepartureAirportId { get; init; }
    public DateTime ArriveDate { get; init; }
    public Guid ArriveAirportId { get; init; }
    public decimal DurationMinutes { get; init; }
    public DateTime FlightDate { get; init; }
    public Enums.FlightStatus Status { get; init; }
    public decimal Price { get; init; }
    public bool IsDeleted { get; init; }
}
