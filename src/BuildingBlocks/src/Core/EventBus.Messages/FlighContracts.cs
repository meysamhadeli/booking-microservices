using Woo.Core.Event;

namespace Woo.Core.EventBus.Messages;

public record FlightCreated(Guid Id) : IIntegrationEvent;
public record FlightUpdated(Guid Id) : IIntegrationEvent;
public record FlightDeleted(Guid Id) : IIntegrationEvent;
public record AircraftCreated(Guid Id) : IIntegrationEvent;
public record AirportCreated(Guid Id) : IIntegrationEvent;
public record SeatCreated(Guid Id) : IIntegrationEvent;
public record SeatReserved(Guid Id) : IIntegrationEvent;