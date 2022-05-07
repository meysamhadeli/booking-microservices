using System.Security.Claims;
using BuildingBlocks.Domain.Event;
using BuildingBlocks.Web;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Domain;

public sealed class BusPublisher : IBusPublisher
{
    private readonly IEventMapper _eventMapper;
    private readonly ILogger<BusPublisher> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BusPublisher(IServiceScopeFactory serviceScopeFactory,
        IEventMapper eventMapper,
        ILogger<BusPublisher> logger,
        IPublishEndpoint publishEndpoint,
        IHttpContextAccessor httpContextAccessor)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _eventMapper = eventMapper;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendAsync(IDomainEvent domainEvent,
        CancellationToken cancellationToken = default) => await SendAsync(new[] { domainEvent }, cancellationToken);

    public async Task SendAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents is null) return;

        _logger.LogTrace("Processing integration events start...");

        var integrationEvents = await MapDomainEventToIntegrationEventAsync(domainEvents).ConfigureAwait(false);

        if (!integrationEvents.Any()) return;

        foreach (var integrationEvent in integrationEvents)
        {
            await _publishEndpoint.Publish((object)integrationEvent, context =>
            {
                context.CorrelationId = new Guid(_httpContextAccessor.HttpContext.GetCorrelationId());
                context.Headers.Set("UserId",
                    _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
                context.Headers.Set("UserName",
                    _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Name));
            }, cancellationToken);

            _logger.LogTrace("Publish a message with ID: {Id}", integrationEvent?.EventId);
        }

        _logger.LogTrace("Processing integration events done...");
    }



    public async Task SendAsync(IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default) => await SendAsync(new[] { integrationEvent }, cancellationToken);

    public async Task SendAsync(IReadOnlyList<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default)
    {
        if (integrationEvents is null) return;

        _logger.LogTrace("Processing integration events start...");

        foreach (var integrationEvent in integrationEvents)
        {
            await _publishEndpoint.Publish((object)integrationEvent, context =>
            {
                context.CorrelationId = new Guid(_httpContextAccessor.HttpContext.GetCorrelationId());
                context.Headers.Set("UserId",
                    _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
                context.Headers.Set("UserName",
                    _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Name));
            }, cancellationToken);

            _logger.LogTrace("Publish a message with ID: {Id}", integrationEvent?.EventId);
        }

        _logger.LogTrace("Processing integration events done...");
    }

    private Task<IReadOnlyList<IIntegrationEvent>> MapDomainEventToIntegrationEventAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        var wrappedIntegrationEvents = GetWrappedIntegrationEvents(events.ToList())?.ToList();
        if (wrappedIntegrationEvents?.Count > 0)
            return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(wrappedIntegrationEvents);

        var integrationEvents = new List<IIntegrationEvent>();
        using var scope = _serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            _logger.LogTrace($"Handling domain event: {eventType.Name}");

            var integrationEvent = _eventMapper.Map(@event);

            if (integrationEvent is null) continue;

            integrationEvents.Add(integrationEvent);
        }

        return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(integrationEvents);
    }

    private IEnumerable<IIntegrationEvent> GetWrappedIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents.Where(x =>
                     x is IHaveIntegrationEvent))
        {
            var genericType = typeof(IntegrationEventWrapper<>)
                .MakeGenericType(domainEvent.GetType());

            var domainNotificationEvent = (IIntegrationEvent)Activator
                .CreateInstance(genericType, domainEvent);

            yield return domainNotificationEvent;
        }
    }
}
