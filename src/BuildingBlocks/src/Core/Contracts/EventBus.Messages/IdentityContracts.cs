using Woo.Core.Event;

namespace Woo.Core.Contracts.EventBus.Messages;

public record UserCreated(Guid Id, string Name, string PassportNumber) : IIntegrationEvent;