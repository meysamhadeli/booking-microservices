using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Domain;

public record IntegrationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) : IIntegrationEvent
    where TDomainEventType : IDomainEvent;
