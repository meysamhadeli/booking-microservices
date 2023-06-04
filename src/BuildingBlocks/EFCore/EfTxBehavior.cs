using System.Text.Json;
using BuildingBlocks.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EFCore;

using System.Transactions;
using PersistMessageProcessor;
using Polly;

public class EfTxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<EfTxBehavior<TRequest, TResponse>> _logger;
    private readonly IDbContext _dbContextBase;
    private readonly IPersistMessageDbContext _persistMessageDbContext;
    private readonly IEventDispatcher _eventDispatcher;

    public EfTxBehavior(
        ILogger<EfTxBehavior<TRequest, TResponse>> logger,
        IDbContext dbContextBase,
        IPersistMessageDbContext persistMessageDbContext,
        IEventDispatcher eventDispatcher)
    {
        _logger = logger;
        _dbContextBase = dbContextBase;
        _persistMessageDbContext = persistMessageDbContext;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "{Prefix} Handled command {MediatrRequest}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        _logger.LogDebug(
            "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request));

        _logger.LogInformation(
            "{Prefix} Open the transaction for {MediatrRequest}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        //ref: https://learn.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
         using var scope = new TransactionScope(TransactionScopeOption.Required,
             new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
             TransactionScopeAsyncFlowOption.Enabled);

        var response = await next();

        _logger.LogInformation(
            "{Prefix} Executed the {MediatrRequest} request",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        while (true)
        {
            var domainEvents = _dbContextBase.GetDomainEvents();

            if (domainEvents is null || !domainEvents.Any())
            {
                return response;
            }

            await _eventDispatcher.SendAsync(domainEvents.ToArray(), typeof(TRequest), cancellationToken);

            // Save data to database with some retry policy in distributed transaction
            await _dbContextBase.RetryOnFailure(async () =>
            {
                await _dbContextBase.SaveChangesAsync(cancellationToken);
            });

            // Save data to database with some retry policy in distributed transaction
            await _persistMessageDbContext.RetryOnFailure(async () =>
            {
                await _persistMessageDbContext.SaveChangesAsync(cancellationToken);
            });

            scope.Complete();

            return response;
        }
    }
}
