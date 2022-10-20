using BuildingBlocks.Core.Event;

namespace Flight.Aircrafts.Features.CreateAircraft.Events.Domain.V1;

public record AircraftCreatedDomainEvent(long Id, string Name, string Model, int ManufacturingYear, bool IsDeleted) : IDomainEvent;
