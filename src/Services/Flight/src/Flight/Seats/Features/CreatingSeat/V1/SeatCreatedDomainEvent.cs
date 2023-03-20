namespace Flight.Seats.Features.CreatingSeat.V1;

using BuildingBlocks.Core.Event;

public record SeatCreatedDomainEvent(long Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId, bool IsDeleted) : IDomainEvent;
