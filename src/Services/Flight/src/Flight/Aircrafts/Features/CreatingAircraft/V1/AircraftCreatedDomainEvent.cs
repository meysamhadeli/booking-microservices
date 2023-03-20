namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

using BuildingBlocks.Core.Event;

public record AircraftCreatedDomainEvent(long Id, string Name, string Model, int ManufacturingYear, bool IsDeleted) : IDomainEvent;
