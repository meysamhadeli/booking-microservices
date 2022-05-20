using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record FlightCreated(long Id) : IIntegrationEvent;
public record FlightUpdated(long Id) : IIntegrationEvent;
public record FlightDeleted(long Id) : IIntegrationEvent;
public record AircraftCreated(long Id) : IIntegrationEvent;
public record AirportCreated(long Id) : IIntegrationEvent;
