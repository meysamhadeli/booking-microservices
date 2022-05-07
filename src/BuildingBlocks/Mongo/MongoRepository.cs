using System.Linq.Expressions;
using BuildingBlocks.Domain.Model;
using MongoDB.Driver;

namespace BuildingBlocks.Mongo;

public class MongoRepository<TEntity, TId> : IMongoRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    private readonly IMongoDbContext _context;
    protected readonly IMongoCollection<TEntity> DbSet;

    public MongoRepository(IMongoDbContext context)
    {
        _context = context;
        DbSet = _context.GetCollection<TEntity>();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return FindOneAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Find(predicate).SingleOrDefaultAsync(cancellationToken: cancellationToken)!;
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(predicate).ToListAsync(cancellationToken: cancellationToken)!;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsQueryable().ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEntity>> RawQuery(
        string query,
        CancellationToken cancellationToken = default,
        params object[] queryParams)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity, new ReplaceOptions(), cancellationToken);

        return entity;
    }

    public Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return DbSet.DeleteOneAsync(e => entities.Any(i => e.Id.Equals(i.Id)), cancellationToken);
    }

    public Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => DbSet.DeleteOneAsync(predicate, cancellationToken);

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return DbSet.DeleteOneAsync(e => e.Id.Equals(entity.Id), cancellationToken);
    }

    public Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return DbSet.DeleteOneAsync(e => e.Id.Equals(id), cancellationToken);
    }
}
