namespace BookingMonolith.Flight.Seats.Dtos;

public record SeatDto(Guid Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, Guid FlightId);
