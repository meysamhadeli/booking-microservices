using System;
using BuildingBlocks.Core.Event;
using Flight.Flights.Models;

namespace Flight.Flights.Features.DeleteFlight.Reads;

public class DeleteFlightMongoCommand : InternalCommand
{
    public DeleteFlightMongoCommand(long Id, string FlightNumber, long AircraftId, DateTime DepartureDate,
        long DepartureAirportId,
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

    public string FlightNumber { get; }
    public long AircraftId { get; }
    public DateTime DepartureDate { get; }
    public long DepartureAirportId { get; }
    public DateTime ArriveDate { get; }
    public long ArriveAirportId { get; }
    public decimal DurationMinutes { get; }
    public DateTime FlightDate { get; }
    public FlightStatus Status { get; }
    public decimal Price { get; }
    public bool IsDeleted { get; }
}
