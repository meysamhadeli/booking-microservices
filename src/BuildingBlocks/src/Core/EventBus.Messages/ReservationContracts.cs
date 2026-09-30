using Woo.Core.Event;

namespace Woo.Core.EventBus.Messages;

public record BookingCreated(Guid Id) : IIntegrationEvent;