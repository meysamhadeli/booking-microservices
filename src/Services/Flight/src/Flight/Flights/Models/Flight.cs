using System;
using BuildingBlocks.Core.Model;

namespace Flight.Flights.Models;

using Features.CreatingFlight.V1;
using Features.DeletingFlight.V1;
using Features.UpdatingFlight.V1;

public record Flight : Aggregate<Guid>
{
    public string FlightNumber { get; private set; }
    public Guid AircraftId { get; private set; }
    public DateTime DepartureDate { get; private set; }
    public Guid DepartureAirportId { get; private set; }
    public DateTime ArriveDate { get; private set; }
    public Guid ArriveAirportId { get; private set; }
    public decimal DurationMinutes { get; private set; }
    public DateTime FlightDate { get; private set; }
    public Enums.FlightStatus Status { get; private set; }
    public decimal Price { get; private set; }

    public static Flight Create(Guid id, string flightNumber, Guid aircraftId,
        Guid departureAirportId, DateTime departureDate, DateTime arriveDate,
        Guid arriveAirportId, decimal durationMinutes, DateTime flightDate, Enums.FlightStatus status,
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


    public void Update(Guid id, string flightNumber, Guid aircraftId,
        Guid departureAirportId, DateTime departureDate, DateTime arriveDate,
        Guid arriveAirportId, decimal durationMinutes, DateTime flightDate, Enums.FlightStatus status,
        decimal price, bool isDeleted = false)
    {
        FlightNumber = flightNumber;
        AircraftId = aircraftId;
        DepartureAirportId = departureAirportId;
        DepartureDate = departureDate;
        arriveDate = ArriveDate;
        ArriveAirportId = arriveAirportId;
        DurationMinutes = durationMinutes;
        FlightDate = flightDate;
        Status = status;
        Price = price;
        IsDeleted = isDeleted;

        var @event = new FlightUpdatedDomainEvent(id, flightNumber, aircraftId, departureDate, departureAirportId,
            arriveDate, arriveAirportId, durationMinutes, flightDate, status, price, isDeleted);

        AddDomainEvent(@event);
    }

    public void Delete(Guid id, string flightNumber, Guid aircraftId,
        Guid departureAirportId, DateTime departureDate, DateTime arriveDate,
        Guid arriveAirportId, decimal durationMinutes, DateTime flightDate, Enums.FlightStatus status,
        decimal price, bool isDeleted = true)
    {
        FlightNumber = flightNumber;
        AircraftId = aircraftId;
        DepartureAirportId = departureAirportId;
        DepartureDate = departureDate;
        arriveDate = ArriveDate;
        ArriveAirportId = arriveAirportId;
        DurationMinutes = durationMinutes;
        FlightDate = flightDate;
        Status = status;
        Price = price;
        IsDeleted = isDeleted;

        var @event = new FlightDeletedDomainEvent(id, flightNumber, aircraftId, departureDate, departureAirportId,
            arriveDate, arriveAirportId, durationMinutes, flightDate, status, price, isDeleted);

        AddDomainEvent(@event);
    }
}
