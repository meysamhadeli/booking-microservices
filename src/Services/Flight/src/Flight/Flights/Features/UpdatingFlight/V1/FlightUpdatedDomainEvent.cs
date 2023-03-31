namespace Flight.Flights.Features.UpdatingFlight.V1;

using System;
using BuildingBlocks.Core.Event;

public record FlightUpdatedDomainEvent(Guid Id, string FlightNumber, Guid AircraftId, DateTime DepartureDate,
    Guid DepartureAirportId, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;
