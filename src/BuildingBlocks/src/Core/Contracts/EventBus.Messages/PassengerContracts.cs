using Woo.Core.Event;

namespace Woo.Core.Contracts.EventBus.Messages;

public record PassengerRegistrationCompleted(Guid Id) : IIntegrationEvent;
public record PassengerCreated(Guid Id) : IIntegrationEvent;