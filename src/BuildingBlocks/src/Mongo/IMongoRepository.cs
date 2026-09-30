using Woo.Core.Model;

namespace Woo.Mongo;

public interface IMongoRepository<TEntity, in TId> : IRepository<TEntity, TId>
    where TEntity : class, IAggregate<TId>
{
}