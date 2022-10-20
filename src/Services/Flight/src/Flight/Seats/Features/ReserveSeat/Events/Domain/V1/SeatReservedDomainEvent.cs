using BuildingBlocks.Core.Event;

namespace Flight.Seats.Features.ReserveSeat.Events.Domain.V1;

public record SeatReservedDomainEvent(long Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId, bool IsDeleted) : IDomainEvent;
