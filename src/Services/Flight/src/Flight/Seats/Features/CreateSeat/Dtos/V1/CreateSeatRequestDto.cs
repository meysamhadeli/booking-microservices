namespace Flight.Seats.Features.CreateSeat.Dtos.V1;

public record CreateSeatRequestDto(string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId);
