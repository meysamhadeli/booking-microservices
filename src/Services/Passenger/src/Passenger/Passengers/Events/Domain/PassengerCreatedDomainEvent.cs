using BuildingBlocks.Core.Event;

namespace Passenger.Passengers.Events.Domain;

public record PassengerCreatedDomainEvent(long Id, string Name, string PassportNumber, bool IsDeleted = false) : IDomainEvent;
