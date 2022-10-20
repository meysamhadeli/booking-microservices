using System;
using BuildingBlocks.Core.Event;

namespace Flight.Flights.Features.UpdateFlight.Events.V1;
public record FlightUpdatedDomainEvent(long Id, string FlightNumber, long AircraftId, DateTime DepartureDate,
    long DepartureAirportId, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;
