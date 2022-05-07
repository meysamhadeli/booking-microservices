using BuildingBlocks.Domain.Model;

namespace BuildingBlocks.Mongo;

public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
}
