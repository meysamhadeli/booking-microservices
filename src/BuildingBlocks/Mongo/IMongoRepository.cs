using BuildingBlocks.Core.Model;

namespace BuildingBlocks.Mongo;

public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IAggregate<TId>
{
}
