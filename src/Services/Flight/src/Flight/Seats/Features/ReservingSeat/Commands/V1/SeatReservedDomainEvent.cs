namespace Flight.Seats.Features.ReservingSeat.Commands.V1;

using BuildingBlocks.Core.Event;

public record SeatReservedDomainEvent(long Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId, bool IsDeleted) : IDomainEvent;
