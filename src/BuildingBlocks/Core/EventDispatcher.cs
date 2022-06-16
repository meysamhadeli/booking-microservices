using System.Security.Claims;
using BuildingBlocks.Core.Event;
using BuildingBlocks.MessageProcessor;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MessageEnvelope = BuildingBlocks.Core.Event.MessageEnvelope;

namespace BuildingBlocks.Core;

public sealed class EventDispatcher : IEventDispatcher
{
    private readonly IEventMapper _eventMapper;
    private readonly ILogger<EventDispatcher> _logger;
    private readonly IPersistMessageProcessor _persistMessageProcessor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventDispatcher(IServiceScopeFactory serviceScopeFactory,
        IEventMapper eventMapper,
        ILogger<EventDispatcher> logger,
        IPersistMessageProcessor persistMessageProcessor,
        IHttpContextAccessor httpContextAccessor)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _eventMapper = eventMapper;
        _logger = logger;
        _persistMessageProcessor = persistMessageProcessor;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendAsync(IDomainEvent domainEvent,
        CancellationToken cancellationToken = default) => await SendAsync(new[] {domainEvent}, cancellationToken);

    public async Task SendAsync(IReadOnlyList<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents is null) return;

        var integrationEvents = await MapDomainEventToIntegrationEventAsync(domainEvents).ConfigureAwait(false);

        if (integrationEvents.Count == 0) return;

        foreach (var integrationEvent in integrationEvents)
        {
            await _persistMessageProcessor.PublishMessageAsync(new MessageEnvelope(integrationEvent, SetHeaders()),
                cancellationToken);
        }
    }


    public async Task SendAsync(IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default) => await SendAsync(new[] {integrationEvent}, cancellationToken);

    public async Task SendAsync(IReadOnlyList<IIntegrationEvent> integrationEvents,
        CancellationToken cancellationToken = default)
    {
        if (integrationEvents is null) return;

        await _persistMessageProcessor.PublishMessageAsync(new MessageEnvelope(integrationEvents, SetHeaders()),
            cancellationToken);
    }

    private Task<IReadOnlyList<IIntegrationEvent>> MapDomainEventToIntegrationEventAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        _logger.LogTrace("Processing integration events start...");

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

        _logger.LogTrace("Processing integration events done...");

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

    private IDictionary<string, object> SetHeaders()
    {
        var headers = new Dictionary<string, object>();
        headers.Add("CorrelationId", _httpContextAccessor?.HttpContext?.GetCorrelationId());
        headers.Add("UserId", _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        headers.Add("UserName", _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Name));

        return headers;
    }
}
