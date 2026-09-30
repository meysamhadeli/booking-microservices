using Woo.EventStoreDB.Events;
using MediatR;

namespace Woo.EventStoreDB.Projections;

public interface IProjectionPublisher
{
    Task PublishAsync<T>(StreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : INotification;

    Task PublishAsync(StreamEvent streamEvent, CancellationToken cancellationToken = default);
}