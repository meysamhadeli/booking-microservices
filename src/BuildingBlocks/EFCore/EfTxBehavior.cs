using System.Text.Json;
using System.Transactions;
using BuildingBlocks.Core;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.Polly;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EFCore;


public class EfTxBehavior<TRequest, TResponse>(
    ILogger<EfTxBehavior<TRequest, TResponse>> logger,
    IDbContext dbContextBase,
    IPersistMessageDbContext persistMessageDbContext,
    IEventDispatcher eventDispatcher
)
    : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull, IRequest<TResponse>
where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{Prefix} Handled command {MediatrRequest}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        logger.LogDebug(
            "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request));

        logger.LogInformation(
            "{Prefix} Open the transaction for {MediatrRequest}",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        //ref: https://learn.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        var response = await next();

        logger.LogInformation(
            "{Prefix} Executed the {MediatrRequest} request",
            nameof(EfTxBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName);

        while (true)
        {
            var domainEvents = dbContextBase.GetDomainEvents();

            if (domainEvents is null || !domainEvents.Any())
            {
                return response;
            }

            await eventDispatcher.SendAsync(domainEvents.ToArray(), typeof(TRequest), cancellationToken);

            // Save data to database with some retry policy in distributed transaction
            await dbContextBase.RetryOnFailure(async () =>
            {
                await dbContextBase.SaveChangesAsync(cancellationToken);
            });

            // Save data to database with some retry policy in distributed transaction
            await persistMessageDbContext.RetryOnFailure(async () =>
            {
                await persistMessageDbContext.SaveChangesAsync(cancellationToken);
            });

            scope.Complete();

            return response;
        }
    }
}