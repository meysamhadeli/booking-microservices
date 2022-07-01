using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core;

public interface IEventDispatcher
{
    public Task SendAsync<T>(IReadOnlyList<T> events, CancellationToken cancellationToken = default)
        where T : IEvent;
    public Task SendAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IEvent;
}
