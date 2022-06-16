using MassTransit;

namespace BuildingBlocks.Core.Event;

[ExcludeFromTopology]
public interface IIntegrationEvent : IEvent
{
}
