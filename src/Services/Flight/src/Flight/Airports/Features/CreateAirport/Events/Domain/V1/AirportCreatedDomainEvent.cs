using BuildingBlocks.Core.Event;

namespace Flight.Airports.Features.CreateAirport.Events.Domain.V1;

public record AirportCreatedDomainEvent(long Id, string Name, string Address, string Code, bool IsDeleted) : IDomainEvent;
