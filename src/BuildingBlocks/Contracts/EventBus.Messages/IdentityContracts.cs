using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record UserCreated(long Id, string Name, string PassportNumber) : IIntegrationEvent;
