using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core;

public interface IEventDispatcher
{
    public Task SendAsync<T>(IReadOnlyList<T> events, EventType eventType = default, CancellationToken cancellationToken = default)
        where T : IEvent;
    public Task SendAsync<T>(T @event, EventType eventType = default, CancellationToken cancellationToken = default)
        where T : IEvent;
}
