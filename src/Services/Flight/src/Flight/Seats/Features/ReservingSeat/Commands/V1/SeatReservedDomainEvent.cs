namespace Flight.Seats.Features.ReservingSeat.Commands.V1;

using System;
using BuildingBlocks.Core.Event;

public record SeatReservedDomainEvent(Guid Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, Guid FlightId, bool IsDeleted) : IDomainEvent;
