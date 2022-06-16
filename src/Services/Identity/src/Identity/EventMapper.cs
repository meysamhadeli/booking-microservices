using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;

namespace Identity;

public sealed class EventMapper : IEventMapper
{
    public IEnumerable<IIntegrationEvent> MapAll(IEnumerable<IDomainEvent> events)
    {
        return events.Select(Map);
    }

    public IIntegrationEvent Map(IDomainEvent @event)
    {
        return @event switch
        {
            _ => null
        };
    }
}