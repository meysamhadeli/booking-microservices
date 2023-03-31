namespace Passenger.Identity.Consumers.RegisteringNewUser.V1;

using BuildingBlocks.Core.Event;

public record PassengerCreatedDomainEvent(Guid Id, string Name, string PassportNumber, bool IsDeleted = false) : IDomainEvent;
