namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

using System;
using BuildingBlocks.Core.Event;

public record AircraftCreatedDomainEvent(Guid Id, string Name, string Model, int ManufacturingYear, bool IsDeleted) : IDomainEvent;
