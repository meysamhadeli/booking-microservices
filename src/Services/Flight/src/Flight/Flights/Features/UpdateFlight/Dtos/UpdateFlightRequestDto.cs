using System;

namespace Flight.Flights.Features.UpdateFlight.Dtos;

public record UpdateFlightRequestDto(long Id, string FlightNumber, long AircraftId, long DepartureAirportId, DateTime DepartureDate, DateTime ArriveDate,
    long ArriveAirportId, decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted);
