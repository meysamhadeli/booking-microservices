using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core;

public class CompositeEventMapper : IEventMapper
{
    private readonly IEnumerable<IEventMapper> _mappers;

    public CompositeEventMapper(IEnumerable<IEventMapper> mappers)
    {
        _mappers = mappers;
    }

    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        foreach (var mapper in _mappers)
        {
            var integrationEvent = mapper.MapToIntegrationEvent(@event);
            if (integrationEvent is not null)
                return integrationEvent;
        }

        return null;
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        foreach (var mapper in _mappers)
        {
            var internalCommand = mapper.MapToInternalCommand(@event);
            if (internalCommand is not null)
                return internalCommand;
        }

        return null;
    }
}
