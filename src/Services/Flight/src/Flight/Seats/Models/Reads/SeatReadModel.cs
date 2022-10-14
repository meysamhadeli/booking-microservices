namespace Flight.Seats.Models.Reads;

public class SeatReadModel
{
    public long Id { get; init; }
    public long SeatId { get; init; }
    public string SeatNumber { get; init; }
    public Enums.SeatType Type { get; init; }
    public Enums.SeatClass Class { get; init; }
    public long FlightId { get; init; }
    public bool IsDeleted { get; init; }
}
