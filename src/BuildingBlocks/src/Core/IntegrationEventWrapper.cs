using Woo.Core.Event;

namespace Woo.Core;

public record IntegrationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) : IIntegrationEvent
    where TDomainEventType : IDomainEvent;