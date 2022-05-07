using BuildingBlocks.Domain.Event;

namespace Flight.Airports.Events;

public record AirportCreatedDomainEvent(long Id, string Name, string Address, string Code) : IDomainEvent;
