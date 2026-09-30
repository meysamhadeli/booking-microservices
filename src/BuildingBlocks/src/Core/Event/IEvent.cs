using MediatR;
using MassTransit;

namespace Woo.Core.Event;

public interface IEvent : INotification
{
    Guid EventId => NewId.NextGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}