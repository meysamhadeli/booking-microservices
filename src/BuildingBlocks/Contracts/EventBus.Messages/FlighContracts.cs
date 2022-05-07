using BuildingBlocks.Domain.Event;

namespace BuildingBlocks.Contracts.EventBus.Messages;

public record FlightCreated(string FlightNumber) : IIntegrationEvent;
public record FlightUpdated(string FlightNumber) : IIntegrationEvent;
public record AircraftCreated(long Id) : IIntegrationEvent;
public record AirportCreated(long Id) : IIntegrationEvent;
