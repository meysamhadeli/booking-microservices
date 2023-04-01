namespace Flight.Seats.Models;

using System;

public class SeatReadModel
{
    public Guid Id { get; init; }
    public Guid SeatId { get; init; }
    public string SeatNumber { get; init; }
    public Enums.SeatType Type { get; init; }
    public Enums.SeatClass Class { get; init; }
    public Guid FlightId { get; init; }
    public bool IsDeleted { get; init; }
}
