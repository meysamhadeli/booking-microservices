namespace Passenger.Identity.Consumers.RegisteringNewUser.V1;

using Woo.Core.Event;

public record PassengerCreatedDomainEvent(Guid Id, string Name, string PassportNumber, bool IsDeleted = false) : IDomainEvent;