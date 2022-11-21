using System;

namespace Flight.Flights.Features.CreateFlight.Dtos.V1;

public record CreateFlightRequestDto(string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price);
