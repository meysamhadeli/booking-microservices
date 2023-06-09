using System;
using BuildingBlocks.Core.Model;

namespace Flight.Flights.Models;

using Aircrafts.ValueObjects;
using Airports.ValueObjects;
using Features.CreatingFlight.V1;
using Features.DeletingFlight.V1;
using Features.UpdatingFlight.V1;
using global::Flight.Flights.Exceptions;
using global::Flight.Flights.ValueObjects;

public record Flight : Aggregate<FlightId>
{
    public FlightNumber? FlightNumber { get; private set; } = default!;
    public AircraftId AircraftId { get; private set; }
    public AirportId? DepartureAirportId { get; private set; }
    public AirportId? ArriveAirportId { get; private set; }
    public DurationMinutes? DurationMinutes { get; private set; }
    public Enums.FlightStatus Status { get; private set; }
    public Price? Price { get; private set; }
    public ArriveDate? ArriveDate { get; private set; }
    public DepartureDate? DepartureDate { get; private set; }
    public FlightDate? FlightDate { get; private set; }

    public static Flight Create(FlightId id, FlightNumber flightNumber, AircraftId aircraftId,
        AirportId departureAirportId, DepartureDate departureDate, ArriveDate arriveDate,
        AirportId arriveAirportId, DurationMinutes durationMinutes, FlightDate flightDate, Enums.FlightStatus status,
        Price price, bool isDeleted = false)
    {
        //SimpleFlightValidate(departureDate.Value, arriveDate.Value, flightDate.Value);

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

        var @event = new FlightCreatedDomainEvent(flight.Id.Value, flight.FlightNumber.Value, flight.AircraftId.Value,
            flight.DepartureDate.Value, flight.DepartureAirportId.Value,
            flight.ArriveDate.Value, flight.ArriveAirportId.Value, flight.DurationMinutes.Value, flight.FlightDate.Value, flight.Status,
            flight.Price.Value, flight.IsDeleted);

        flight.AddDomainEvent(@event);

        return flight;
    }


    public void Update(FlightId id, FlightNumber flightNumber, AircraftId aircraftId,
        AirportId departureAirportId, DepartureDate departureDate, ArriveDate arriveDate,
        AirportId arriveAirportId, DurationMinutes durationMinutes, FlightDate flightDate, Enums.FlightStatus status,
        Price price, bool isDeleted = false)
    {
        FlightNumber = flightNumber;
        AircraftId = aircraftId;
        DepartureAirportId = departureAirportId;
        DepartureDate = departureDate;
        ArriveDate = arriveDate;
        ArriveAirportId = arriveAirportId;
        DurationMinutes = durationMinutes;
        FlightDate = flightDate;
        Status = status;
        Price = price;
        IsDeleted = isDeleted;

        var @event = new FlightUpdatedDomainEvent(id.Value, flightNumber.Value, aircraftId.Value, departureDate.Value, departureAirportId.Value,
            arriveDate.Value, arriveAirportId.Value, durationMinutes.Value, flightDate.Value, status, price.Value, isDeleted);

        AddDomainEvent(@event);
    }

    public void Delete(FlightId id, FlightNumber flightNumber, AircraftId aircraftId,
        AirportId departureAirportId, DepartureDate departureDate, ArriveDate arriveDate,
        AirportId arriveAirportId, DurationMinutes durationMinutes, FlightDate flightDate, Enums.FlightStatus status,
        Price price, bool isDeleted = true)
    {
        FlightNumber = flightNumber;
        AircraftId = aircraftId;
        DepartureAirportId = departureAirportId;
        DepartureDate = departureDate;
        ArriveDate = arriveDate;
        ArriveAirportId = arriveAirportId;
        DurationMinutes = durationMinutes;
        FlightDate = flightDate;
        Status = status;
        Price = price;
        IsDeleted = isDeleted;

        var @event = new FlightDeletedDomainEvent(id.Value, flightNumber.Value, aircraftId.Value, departureDate.Value, departureAirportId.Value,
            arriveDate.Value, arriveAirportId.Value, durationMinutes.Value, flightDate.Value, status, price.Value, isDeleted);

        AddDomainEvent(@event);
    }

    public static void SimpleFlightValidate(DateTime departureDate, DateTime arriveDate, DateTime flightDate)
    {
        if (departureDate >= arriveDate)
        {
            throw new FlightExceptions(departureDate, arriveDate);
        }

        if (flightDate < departureDate || flightDate > arriveDate)
        {
            throw new FlightExceptions(flightDate);
        }
    }

    public static TimeSpan GetFlightDuration(DateTime departureDate, DateTime arriveDate)
    {
        return arriveDate - departureDate;
    }

    public static bool IsOnSameDay(DateTime departureDate, DateTime arriveDate)
    {
        return departureDate.Date == arriveDate.Date;
    }
}
