namespace Flight.Airports.Features.CreatingAirport.V1;

using BuildingBlocks.Core.Event;

public record AirportCreatedDomainEvent(long Id, string Name, string Address, string Code, bool IsDeleted) : IDomainEvent;
