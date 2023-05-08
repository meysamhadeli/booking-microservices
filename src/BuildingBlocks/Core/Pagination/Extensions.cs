namespace BuildingBlocks.Core.Pagination;

using Sieve.Models;
using Sieve.Services;

public static class Extensions
{
    public static async Task<IPageList<TEntity>> ApplyPagingAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        IPageRequest pageRequest,
        ISieveProcessor sieveProcessor,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
    {
        var sieveModel = new SieveModel
        {
            PageSize = pageRequest.PageSize,
            Page = pageRequest.PageNumber,
            Sorts = pageRequest.SortOrder,
            Filters = pageRequest.Filters
        };

        // https://github.com/Biarity/Sieve/issues/34#issuecomment-403817573
        var result = sieveProcessor.Apply(sieveModel, queryable, applyPagination: false);
        var total = result.Count();
        result = sieveProcessor.Apply(sieveModel, queryable, applyFiltering: false,
            applySorting: false); // Only applies pagination

        var items = await result
            .ToAsyncEnumerable()
            .ToListAsync(cancellationToken: cancellationToken);

        return PageList<TEntity>.Create(items.AsReadOnly(), pageRequest.PageNumber, pageRequest.PageSize, total);
    }
}
