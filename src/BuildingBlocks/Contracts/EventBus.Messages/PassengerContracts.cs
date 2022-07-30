using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record PassengerRegistrationCompleted(long Id) : IIntegrationEvent;
public record PassengerCreated(long Id) : IIntegrationEvent;
