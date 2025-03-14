using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record PassengerRegistrationCompleted(Guid Id) : IIntegrationEvent;
public record PassengerCreated(Guid Id) : IIntegrationEvent;
