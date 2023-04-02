namespace Flight.Seats.Models;

using System;

public class SeatReadModel
{
    public required Guid Id { get; init; }
    public required Guid SeatId { get; init; }
    public required string SeatNumber { get; init; }
    public required Enums.SeatType Type { get; init; }
    public required Enums.SeatClass Class { get; init; }
    public required Guid FlightId { get; init; }
    public required bool IsDeleted { get; init; }
}
