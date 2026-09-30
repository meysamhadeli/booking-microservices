using Woo.Core.Event;

namespace Woo.Core.EventBus.Messages;

public record UserCreated(Guid Id, string Name, string PassportNumber) : IIntegrationEvent;