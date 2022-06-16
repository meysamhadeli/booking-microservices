using BuildingBlocks.Core.Event;

namespace Flight.Aircrafts.Events;

public record AircraftCreatedDomainEvent(long Id, string Name, string Model, int ManufacturingYear) : IDomainEvent;
