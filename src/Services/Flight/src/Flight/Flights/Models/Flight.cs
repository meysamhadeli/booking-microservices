using System;
using BuildingBlocks.Domain.Model;
using Flight.Flights.Events.Domain;

namespace Flight.Flights.Models;

public class Flight : Aggregate<long>
{
    public string FlightNumber { get; private set; }
    public long AircraftId { get; private set; }
    public DateTime DepartureDate { get; private set; }
    public long DepartureAirportId { get; private set; }
    public DateTime ArriveDate { get; private set; }
    public long ArriveAirportId { get; private set; }
    public decimal DurationMinutes { get; private set; }
    public DateTime FlightDate { get; private set; }
    public FlightStatus Status { get; private set; }
    public decimal Price { get; private set; }

    public static Flight Create(long id, string flightNumber, long aircraftId,
        long departureAirportId, DateTime departureDate, DateTime arriveDate,
        long arriveAirportId, decimal durationMinutes, DateTime flightDate, FlightStatus status,
        decimal price, bool isDeleted = false)
    {
        var flight = new Flight
        {
            Id = id,
            FlightNumber = flightNumber,
            AircraftId = aircraftId,
            DepartureAirportId = departureAirportId,
            DepartureDate = departureDate,
            ArriveDate = arriveDate,
            ArriveAirportId = arriveAirportId,
            DurationMinutes = durationMinutes,
            FlightDate = flightDate,
            Status = status,
            Price = price,
            IsDeleted = isDeleted,
        };

        var @event = new FlightCreatedDomainEvent(flight.Id, flight.FlightNumber, flight.AircraftId,
            flight.DepartureDate, flight.DepartureAirportId,
            flight.ArriveDate, flight.ArriveAirportId, flight.DurationMinutes, flight.FlightDate, flight.Status,
            flight.Price, flight.IsDeleted);

        flight.AddDomainEvent(@event);

        return flight;
    }


    public void Update(long id, string flightNumber, long aircraftId,
        long departureAirportId, DateTime departureDate, DateTime arriveDate,
        long arriveAirportId, decimal durationMinutes, DateTime flightDate, FlightStatus status,
        decimal price, bool isDeleted = false)
    {
        var @event = new FlightUpdatedDomainEvent(id, flightNumber, aircraftId, departureDate, departureAirportId,
            arriveDate, arriveAirportId, durationMinutes, flightDate, status, price, isDeleted);

        AddDomainEvent(@event);
    }
}
