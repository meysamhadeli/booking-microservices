using System;

namespace Flight.Flights.Dtos;

public record FlightDto(Guid Id, string FlightNumber, Guid AircraftId, Guid DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, decimal Price);
