using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record BookingCreated(long Id) : IIntegrationEvent;
