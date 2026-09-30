using Woo.Core.Event;
using MediatR;

namespace Woo.EventStoreDB.Events;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent
{
}