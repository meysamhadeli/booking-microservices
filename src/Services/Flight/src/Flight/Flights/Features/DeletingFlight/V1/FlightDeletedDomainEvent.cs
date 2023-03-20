namespace Flight.Flights.Features.DeletingFlight.V1;

using System;
using BuildingBlocks.Core.Event;

public record FlightDeletedDomainEvent(long Id, string FlightNumber, long AircraftId, DateTime DepartureDate,
    long DepartureAirportId, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;
