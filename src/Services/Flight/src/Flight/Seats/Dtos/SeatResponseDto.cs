namespace Flight.Seats.Dtos;

public record SeatResponseDto(long Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId);
