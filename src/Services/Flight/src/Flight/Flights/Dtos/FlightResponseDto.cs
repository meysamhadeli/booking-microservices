using System;
using Flight.Flights.Models;

namespace Flight.Flights.Dtos;

public record FlightResponseDto(long Id, string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, decimal Price);
