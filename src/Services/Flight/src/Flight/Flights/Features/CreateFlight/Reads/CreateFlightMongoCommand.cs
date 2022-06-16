using System;
using BuildingBlocks.Core.Event;
using Flight.Flights.Models;

namespace Flight.Flights.Features.CreateFlight.Reads;

public class CreateFlightMongoCommand : InternalCommand
{
    public CreateFlightMongoCommand()
    {

    }

    public CreateFlightMongoCommand(long Id, string FlightNumber, long AircraftId, DateTime DepartureDate, long DepartureAirportId,
        DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes, DateTime FlightDate, FlightStatus Status,
        decimal Price, bool IsDeleted)
    {
        this.Id = Id;
        this.FlightNumber = FlightNumber;
        this.AircraftId = AircraftId;
        this.DepartureDate = DepartureDate;
        this.DepartureAirportId = DepartureAirportId;
        this.ArriveDate = ArriveDate;
        this.ArriveAirportId = ArriveAirportId;
        this.DurationMinutes = DurationMinutes;
        this.FlightDate = FlightDate;
        this.Status = Status;
        this.Price = Price;
        this.IsDeleted = IsDeleted;
    }

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
    public bool IsDeleted { get; init; }
}
