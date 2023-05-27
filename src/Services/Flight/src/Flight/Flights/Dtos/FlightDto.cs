using System;

namespace Flight.Flights.Dtos;

using Aircrafts.Models.ValueObjects;

public record FlightDto(Guid Id, string FlightNumber, AircraftId AircraftId, Guid DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, decimal Price);
