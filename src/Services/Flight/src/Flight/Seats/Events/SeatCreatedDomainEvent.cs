using BuildingBlocks.Core.Event;
using Flight.Seats.Models;

namespace Flight.Seats.Events;

public record SeatCreatedDomainEvent(long Id, string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId, bool IsDeleted) : IDomainEvent;
