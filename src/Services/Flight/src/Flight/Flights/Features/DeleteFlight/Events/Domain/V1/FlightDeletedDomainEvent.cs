using System;
using BuildingBlocks.Core.Event;

namespace Flight.Flights.Features.DeleteFlight.Events.Domain.V1;

public record FlightDeletedDomainEvent(long Id, string FlightNumber, long AircraftId, DateTime DepartureDate,
    long DepartureAirportId, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;
