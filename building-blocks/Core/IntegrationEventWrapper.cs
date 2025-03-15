using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Core;

public record IntegrationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) : IIntegrationEvent
    where TDomainEventType : IDomainEvent;
