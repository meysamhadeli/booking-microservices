using System.Text.Json;
using System.Transactions;
using BuildingBlocks.Core;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.Polly;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingMonolith.Flight.Data;

public class EfTxFlightBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull, IRequest<TResponse>
where TResponse : notnull
{
    private readonly ILogger<EfTxFlightBehavior<TRequest, TResponse>> _logger;
    private readonly FlightDbContext _flightDbContext;
    private readonly IPersistMessageDbContext _persistMessageDbContext;
    private readonly IEventDispatcher _eventDispatcher;

    public EfTxFlightBehavior(
        ILogger<EfTxFlightBehavior<TRequest, TResponse>> logger,
        FlightDbContext flightDbContext,
        IPersistMessageDbContext persistMessageDbContext,
        IEventDispatcher eventDispatcher
    )
    {
        _logger = logger;
        _flightDbContext = flightDbContext;
        _persistMessageDbContext = persistMessageDbContext;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "{Prefix} Handled command {MediatrRequest}",
            GetType().Name,
            typeof(TRequest).FullName);

        _logger.LogDebug(
            "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
            GetType().Name,
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request));

        var response = await next();

        _logger.LogInformation(
            "{Prefix} Executed the {MediatrRequest} request",
            GetType().Name,
            typeof(TRequest).FullName);

        while (true)
        {
            var domainEvents = _flightDbContext.GetDomainEvents();

            if (domainEvents is null || !domainEvents.Any())
            {
                return response;
            }

            _logger.LogInformation(
                "{Prefix} Open the transaction for {MediatrRequest}",
                GetType().Name,
                typeof(TRequest).FullName);

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled);

            await _eventDispatcher.SendAsync(
                domainEvents.ToArray(),
                typeof(TRequest),
                cancellationToken);

            // Save data to database with some retry policy in distributed transaction
            await _flightDbContext.RetryOnFailure(
                async () =>
                {
                    await _flightDbContext.SaveChangesAsync(cancellationToken);
                });

            // Save data to database with some retry policy in distributed transaction
            await _persistMessageDbContext.RetryOnFailure(
                async () =>
                {
                    await _persistMessageDbContext.SaveChangesAsync(cancellationToken);
                });

            scope.Complete();

            return response;
        }
    }
}
