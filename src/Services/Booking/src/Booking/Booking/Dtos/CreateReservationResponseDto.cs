namespace Booking.Booking.Dtos;

public record BookingResponseDto(long Id, string Name, string FlightNumber, long AircraftId, decimal Price,
    DateTime FlightDate, string SeatNumber, long DepartureAirportId, long ArriveAirportId, string Description);
