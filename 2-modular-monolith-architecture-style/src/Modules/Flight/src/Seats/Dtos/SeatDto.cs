namespace Flight.Seats.Dtos;

using System;

public record SeatDto(Guid Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, Guid FlightId);
