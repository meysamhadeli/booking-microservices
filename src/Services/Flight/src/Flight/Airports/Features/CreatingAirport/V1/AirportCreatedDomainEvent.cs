namespace Flight.Airports.Features.CreatingAirport.V1;

using System;
using BuildingBlocks.Core.Event;

public record AirportCreatedDomainEvent(Guid Id, string Name, string Address, string Code, bool IsDeleted) : IDomainEvent;
