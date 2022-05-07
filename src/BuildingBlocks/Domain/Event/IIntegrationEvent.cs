using MassTransit;
using MassTransit.Topology;

namespace BuildingBlocks.Domain.Event;

[ExcludeFromTopology]
public interface IIntegrationEvent : IEvent
{
}
