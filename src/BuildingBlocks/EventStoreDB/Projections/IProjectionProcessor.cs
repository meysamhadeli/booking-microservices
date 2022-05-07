using BuildingBlocks.EventStoreDB.Events;
using MediatR;

namespace BuildingBlocks.EventStoreDB.Projections;

public interface IProjectionProcessor
{
    Task ProcessEventAsync<T>(StreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : INotification;
}
