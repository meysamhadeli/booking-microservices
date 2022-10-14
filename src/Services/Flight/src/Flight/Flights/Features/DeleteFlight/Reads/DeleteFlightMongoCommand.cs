using System;
using BuildingBlocks.Core.Event;
using Flight.Flights.Models;

namespace Flight.Flights.Features.DeleteFlight.Reads;

public class DeleteFlightMongoCommand : InternalCommand
{
    public DeleteFlightMongoCommand(long id, string flightNumber, long aircraftId, DateTime departureDate,
        long departureAirportId,
        DateTime arriveDate, long arriveAirportId, decimal durationMinutes, DateTime flightDate, Enums.FlightStatus status,
        decimal price, bool isDeleted)
    {
        Id = id;
        FlightNumber = flightNumber;
        AircraftId = aircraftId;
        DepartureDate = departureDate;
        DepartureAirportId = departureAirportId;
        ArriveDate = arriveDate;
        ArriveAirportId = arriveAirportId;
        DurationMinutes = durationMinutes;
        FlightDate = flightDate;
        Status = status;
        Price = price;
        IsDeleted = isDeleted;
    }

    public string FlightNumber { get; }
    public long AircraftId { get; }
    public DateTime DepartureDate { get; }
    public long DepartureAirportId { get; }
    public DateTime ArriveDate { get; }
    public long ArriveAirportId { get; }
    public decimal DurationMinutes { get; }
    public DateTime FlightDate { get; }
    public Enums.FlightStatus Status { get; }
    public decimal Price { get; }
    public bool IsDeleted { get; }
}
