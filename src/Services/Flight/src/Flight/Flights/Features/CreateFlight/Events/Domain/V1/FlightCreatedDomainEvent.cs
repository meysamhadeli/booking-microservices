using System;
using BuildingBlocks.Core.Event;

namespace Flight.Flights.Features.CreateFlight.Events.Domain.V1;

public record FlightCreatedDomainEvent(long Id, string FlightNumber, long AircraftId, DateTime DepartureDate,
    long DepartureAirportId, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;
