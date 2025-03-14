namespace BuildingBlocks.Mongo;

public class MongoUnitOfWork<TContext> : IMongoUnitOfWork<TContext>, ITransactionAble
    where TContext : MongoDbContext
{
    public MongoUnitOfWork(TContext context) => Context = context;

    public TContext Context { get; }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.BeginTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.RollbackTransaction(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.CommitTransactionAsync(cancellationToken);
    }

    public void Dispose() => Context.Dispose();
}

