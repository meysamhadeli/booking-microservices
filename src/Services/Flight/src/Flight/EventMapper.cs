using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Event;
using Flight.Aircrafts.Events;
using Flight.Airports.Events;
using Flight.Flights.Events.Domain;

namespace Flight;

// ref: https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public sealed class EventMapper : IEventMapper
{
    public IEnumerable<IIntegrationEvent> MapAll(IEnumerable<IDomainEvent> events) => events.Select(Map);

    public IIntegrationEvent Map(IDomainEvent @event)
    {
        return @event switch
        {
            FlightCreatedDomainEvent e => new FlightCreated(e.FlightNumber),
            FlightUpdatedDomainEvent e => new FlightUpdated(e.FlightNumber),
            AirportCreatedDomainEvent e => new AirportCreated(e.Id),
            AircraftCreatedDomainEvent e => new AircraftCreated(e.Id),
            _ => null
        };
    }
}
