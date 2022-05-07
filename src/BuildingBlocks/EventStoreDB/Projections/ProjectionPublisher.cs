using BuildingBlocks.EventStoreDB.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EventStoreDB.Projections;

public class ProjectionPublisher : IProjectionPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public ProjectionPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<T>(StreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : INotification
    {
        using var scope = _serviceProvider.CreateScope();
        var projectionsProcessors = scope.ServiceProvider.GetRequiredService<IEnumerable<IProjectionProcessor>>();
        foreach (var projectionProcessor in projectionsProcessors)
        {
            await projectionProcessor.ProcessEventAsync(streamEvent, cancellationToken);
        }
    }

    public Task PublishAsync(StreamEvent streamEvent, CancellationToken cancellationToken = default)
    {
        var streamData = streamEvent.Data.GetType();

        var method = typeof(IProjectionPublisher)
            .GetMethods()
            .Single(m => m.Name == nameof(PublishAsync) && m.GetGenericArguments().Any())
            .MakeGenericMethod(streamData);

        return (Task)method
            .Invoke(this, new object[] { streamEvent, cancellationToken })!;
    }
}
