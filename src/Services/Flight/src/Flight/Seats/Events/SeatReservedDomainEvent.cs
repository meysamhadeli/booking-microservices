using BuildingBlocks.Core.Event;
using Flight.Seats.Models;

namespace Flight.Seats.Events;

public record SeatReservedDomainEvent(long Id, string SeatNumber, SeatType Type, SeatClass Class, long FlightId, bool IsDeleted) : IDomainEvent;
