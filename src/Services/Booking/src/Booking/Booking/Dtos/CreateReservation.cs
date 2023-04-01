namespace Booking.Booking.Dtos;

public record BookingResponseDto(Guid Id, string Name, string FlightNumber, Guid AircraftId, decimal Price,
    DateTime FlightDate, string SeatNumber, Guid DepartureAirportId, Guid ArriveAirportId, string Description);
