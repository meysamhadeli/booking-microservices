using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record BookingCreated(Guid Id) : IIntegrationEvent;
