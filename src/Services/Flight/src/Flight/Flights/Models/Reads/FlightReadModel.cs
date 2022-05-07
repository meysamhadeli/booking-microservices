using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flight.Flights.Models.Reads;

public class FlightReadModel
{
    public long Id { get; init; }
    public string FlightNumber { get; init; }
    public long AircraftId { get; init; }
    public DateTime DepartureDate { get; init; }
    public long DepartureAirportId { get; init; }
    public DateTime ArriveDate { get; init; }
    public long ArriveAirportId { get; init; }
    public decimal DurationMinutes { get; init; }
    public DateTime FlightDate { get; init; }
    public FlightStatus Status { get; init; }
    public decimal Price { get; init; }
    public bool IsDeleted { get; set; }
}
