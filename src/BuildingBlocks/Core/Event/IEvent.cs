using BuildingBlocks.IdsGenerator;
using MediatR;

namespace BuildingBlocks.Core.Event;

public interface IEvent : INotification
{
    long EventId => SnowFlakIdGenerator.NewId();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}
