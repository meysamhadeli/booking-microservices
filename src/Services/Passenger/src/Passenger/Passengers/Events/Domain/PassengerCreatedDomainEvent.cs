using BuildingBlocks.Domain.Event;

namespace Passenger.Passengers.Events.Domain;

public record PassengerCreatedDomainEvent(string FlightNumber) : IDomainEvent;
