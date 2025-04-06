using BuildingBlocks.Core.Model;

namespace Flight.Flights.Models;

using Aircrafts.ValueObjects;
using Airports.ValueObjects;
using Features.CreatingFlight.V1;
using Features.DeletingFlight.V1;
using Features.UpdatingFlight.V1;
using ValueObjects;

public record Flight : Aggregate<FlightId>
{
    public FlightNumber FlightNumber { get; private set; } = default!;
    public AircraftId AircraftId { get; private set; } = default!;
    public AirportId DepartureAirportId { get; private set; } = default!;
    public AirportId ArriveAirportId { get; private set; } = default!;
    public DurationMinutes DurationMinutes { get; private set; } = default!;
    public Enums.FlightStatus Status { get; private set; }
    public Price Price { get; private set; } = default!;
    public ArriveDate ArriveDate { get; private set; } = default!;
    public DepartureDate DepartureDate { get; private set; } = default!;
    public FlightDate FlightDate { get; private set; } = default!;

    public static Flight Create(FlightId id, FlightNumber flightNumber, AircraftId aircraftId,
        AirportId departureAirportId, DepartureDate departureDate, ArriveDate arriveDate,
        AirportId arriveAirportId, DurationMinutes durationMinutes, FlightDate flightDate, Enums.FlightStatus status,
        Price price, bool isDeleted = false)
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


    public void Update(FlightId id, FlightNumber flightNumber, AircraftId aircraftId,
        AirportId departureAirportId, DepartureDate departureDate, ArriveDate arriveDate,
        AirportId arriveAirportId, DurationMinutes durationMinutes, FlightDate flightDate, Enums.FlightStatus status,
        Price price, bool isDeleted = false)
    {
        this.FlightNumber = flightNumber;
        this.AircraftId = aircraftId;
        this.DepartureAirportId = departureAirportId;
        this.DepartureDate = departureDate;
        this.ArriveDate = arriveDate;
        this.ArriveAirportId = arriveAirportId;
        this.DurationMinutes = durationMinutes;
        this.FlightDate = flightDate;
        this.Status = status;
        this.Price = price;
        this.IsDeleted = isDeleted;

        var @event = new FlightUpdatedDomainEvent(id, flightNumber, aircraftId, departureDate, departureAirportId,
            arriveDate, arriveAirportId, durationMinutes, flightDate, status, price, isDeleted);

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

        var @event = new FlightDeletedDomainEvent(id, flightNumber, aircraftId, departureDate, departureAirportId,
            arriveDate, arriveAirportId, durationMinutes, flightDate, status, price, isDeleted);

        AddDomainEvent(@event);
    }
}
