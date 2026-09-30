using System.Security.Claims;
using System.Text.Json;
using Woo.Core.Event;
using Woo.Web;
using Woo.Wolverine;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using global::Wolverine;
using MessageEnvelope = Woo.Core.Event.MessageEnvelope;

namespace Woo.Core;

public sealed class EventDispatcher(
    IServiceScopeFactory serviceScopeFactory,
    IEventMapper eventMapper,
    ILogger<EventDispatcher> logger,
    IMessageBus messageBus,
    IHttpContextAccessor httpContextAccessor
)
    : IEventDispatcher
{
    public async Task SendAsync<T>(IReadOnlyList<T> events, Type type = null,
                                   CancellationToken cancellationToken = default)
        where T : IEvent
    {
        if (events.Count > 0)
        {
            var eventType = type != null && type.IsAssignableTo(typeof(IInternalCommand))
                ? EventType.InternalCommand
                : EventType.DomainEvent;

            async Task PublishIntegrationEvent(IReadOnlyList<IIntegrationEvent> integrationEvents)
            {
                var deliveryOptions = BuildDeliveryOptions();

                foreach (var integrationEvent in integrationEvents)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await messageBus.PublishAsync(integrationEvent, deliveryOptions);
                }
            }

            switch (events)
            {
                case IReadOnlyList<IDomainEvent> domainEvents:
                    {
                        var integrationEvents = await MapDomainEventToIntegrationEventAsync(domainEvents)
                        .ConfigureAwait(false);

                        await PublishIntegrationEvent(integrationEvents);
                        break;
                    }

                case IReadOnlyList<IIntegrationEvent> integrationEvents:
                    await PublishIntegrationEvent(integrationEvents);
                    break;
            }

            if (type != null && eventType == EventType.InternalCommand)
            {
                var internalMessages = await MapDomainEventToInternalCommandAsync(events as IReadOnlyList<IDomainEvent>)
                    .ConfigureAwait(false);

                var deliveryOptions = BuildDeliveryOptions();

                foreach (var internalMessage in internalMessages)
                {
                    await messageBus.SendAsync(
                        new DurableInternalCommand(
                            internalMessage.GetType().ToString(),
                            JsonSerializer.Serialize(internalMessage, internalMessage.GetType())),
                        deliveryOptions);
                }
            }
        }
    }

    public async Task SendAsync<T>(T @event, Type type = null,
        CancellationToken cancellationToken = default)
        where T : IEvent =>
        await SendAsync(new[] { @event }, type, cancellationToken);


    private Task<IReadOnlyList<IIntegrationEvent>> MapDomainEventToIntegrationEventAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        logger.LogTrace("Processing integration events start...");

        var wrappedIntegrationEvents = GetWrappedIntegrationEvents(events.ToList())?.ToList();
        if (wrappedIntegrationEvents?.Count > 0)
            return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(wrappedIntegrationEvents);

        var integrationEvents = new List<IIntegrationEvent>();
        using var scope = serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            logger.LogTrace($"Handling domain event: {eventType.Name}");

            var integrationEvent = eventMapper.MapToIntegrationEvent(@event);

            if (integrationEvent is null)
                continue;

            integrationEvents.Add(integrationEvent);
        }

        logger.LogTrace("Processing integration events done...");

        return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(integrationEvents);
    }


    private Task<IReadOnlyList<IInternalCommand>> MapDomainEventToInternalCommandAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        logger.LogTrace("Processing internal message start...");

        var internalCommands = new List<IInternalCommand>();
        using var scope = serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            logger.LogTrace($"Handling domain event: {eventType.Name}");

            var integrationEvent = eventMapper.MapToInternalCommand(@event);

            if (integrationEvent is null)
                continue;

            internalCommands.Add(integrationEvent);
        }

        logger.LogTrace("Processing internal message done...");

        return Task.FromResult<IReadOnlyList<IInternalCommand>>(internalCommands);
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
        headers.Add("CorrelationId", httpContextAccessor?.HttpContext?.GetCorrelationId());
        headers.Add("UserId", httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        headers.Add("UserName", httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Name));

        return headers;
    }

    private DeliveryOptions BuildDeliveryOptions()
    {
        var deliveryOptions = new DeliveryOptions();

        foreach (var header in SetHeaders())
        {
            deliveryOptions.WithHeader(header.Key, header.Value?.ToString() ?? string.Empty);
        }

        return deliveryOptions;
    }
}