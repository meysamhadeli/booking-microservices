namespace Booking.Booking.Models.ValueObjects;

public record Trip(string FlightNumber, long AircraftId, long DepartureAirportId, long ArriveAirportId,
    DateTime FlightDate, decimal Price, string Description, string SeatNumber);
