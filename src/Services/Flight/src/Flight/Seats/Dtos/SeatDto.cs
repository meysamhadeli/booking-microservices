namespace Flight.Seats.Dtos;

public record SeatDto(long Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId);
