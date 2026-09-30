using Woo.Core.Event;

namespace Woo.Core.EventBus.Messages;

public record PassengerRegistrationCompleted(Guid Id) : IIntegrationEvent;
public record PassengerCreated(Guid Id) : IIntegrationEvent;