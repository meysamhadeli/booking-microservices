namespace Booking.Booking.Models.ValueObjects;

public record Trip(string FlightNumber, Guid AircraftId, Guid DepartureAirportId, Guid ArriveAirportId,
    DateTime FlightDate, decimal Price, string Description, string SeatNumber);
