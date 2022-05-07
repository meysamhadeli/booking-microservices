using BuildingBlocks.Domain.Model;
using BuildingBlocks.Exception;

namespace BuildingBlocks.EventStoreDB.Repository;

public static class RepositoryExtensions
{
    public static async Task<T> Get<T>(
        this IEventStoreDBRepository<T> repository,
        long id,
        CancellationToken cancellationToken
    ) where T : class, IAggregate<long>
    {
        var entity = await repository.Find(id, cancellationToken);

        return entity ?? throw AggregateNotFoundException.For<T>(id);
    }

    public static async Task<ulong> GetAndUpdate<T>(
        this IEventStoreDBRepository<T> repository,
        long id,
        Action<T> action,
        long? expectedVersion = null,
        CancellationToken cancellationToken = default
    ) where T : class, IAggregate<long>
    {
        var entity = await repository.Get(id, cancellationToken);

        action(entity);

        return await repository.Update(entity, expectedVersion, cancellationToken);
    }
}
