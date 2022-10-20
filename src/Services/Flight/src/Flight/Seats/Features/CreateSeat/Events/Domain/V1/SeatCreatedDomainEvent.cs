using BuildingBlocks.Core.Event;

namespace Flight.Seats.Features.CreateSeat.Events.Domain.V1;

public record SeatCreatedDomainEvent(long Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId, bool IsDeleted) : IDomainEvent;
