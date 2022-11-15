using Flight.Seats.Models;

namespace Flight.Seats.Dtos;

public record SeatResponseDto
{
    public long Id { get; set; }
    public string SeatNumber { get; init; }
    public Enums.SeatType Type { get; init; }
    public Enums.SeatClass Class { get; init; }
    public long FlightId { get; init; }
}
