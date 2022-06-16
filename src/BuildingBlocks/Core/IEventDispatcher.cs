using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core;

public interface IEventDispatcher
{
    public Task SendAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    public Task SendAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

    public Task SendAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    public Task SendAsync(IReadOnlyList<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default);
}
