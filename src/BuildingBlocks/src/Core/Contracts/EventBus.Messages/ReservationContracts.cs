using Woo.Core.Event;

namespace Woo.Core.Contracts.EventBus.Messages;

public record BookingCreated(Guid Id) : IIntegrationEvent;