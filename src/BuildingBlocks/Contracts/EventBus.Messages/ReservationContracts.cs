using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record BookingCreated(long Id) : IIntegrationEvent;
