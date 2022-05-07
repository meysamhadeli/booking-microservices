using System.Data;
using System.Text.Json;
using BuildingBlocks.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EFCore;

public class EfTxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<EfTxBehavior<TRequest, TResponse>> _logger;
    private readonly IDbContext _dbContextBase;
    private readonly IBusPublisher _busPublisher;

    public EfTxBehavior(
        ILogger<EfTxBehavior<TRequest, TResponse>> logger,
        IDbContext dbContextBase,
        IBusPublisher busPublisher)
    {
        _logger = logger;
        _dbContextBase = dbContextBase;
        _busPublisher = busPublisher;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
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

        await _dbContextBase.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        try
        {
            var response = await next();

            _logger.LogInformation(
                "{Prefix} Executed the {MediatrRequest} request",
                nameof(EfTxBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName);

            var domainEvents = _dbContextBase.GetDomainEvents();

            await _busPublisher.SendAsync(domainEvents.ToArray(), cancellationToken);

            await _dbContextBase.CommitTransactionAsync(cancellationToken);

            return response;
        }
        catch
        {
            await _dbContextBase.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
