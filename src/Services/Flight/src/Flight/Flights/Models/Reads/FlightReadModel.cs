using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Flight.Flights.Models.Reads;

public class FlightReadModel
{
    public long Id { get; set; }
    public long FlightId { get; set; }
    public string FlightNumber { get; set; }
    public long AircraftId { get; set; }
    public DateTime DepartureDate { get; set; }
    public long DepartureAirportId { get; set; }
    public DateTime ArriveDate { get; set; }
    public long ArriveAirportId { get; set; }
    public decimal DurationMinutes { get; set; }
    public DateTime FlightDate { get; set; }
    public FlightStatus Status { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; }
}
