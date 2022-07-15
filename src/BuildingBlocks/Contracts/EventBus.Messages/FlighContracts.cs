using BuildingBlocks.Core.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record FlightCreated(long Id) : IIntegrationEvent;
public record FlightUpdated(long Id) : IIntegrationEvent;
public record FlightDeleted(long Id) : IIntegrationEvent;
public record AircraftCreated(long Id) : IIntegrationEvent;
public record AirportCreated(long Id) : IIntegrationEvent;
public record SeatCreated(long Id) : IIntegrationEvent;
public record SeatReserved(long Id) : IIntegrationEvent;
