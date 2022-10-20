using BuildingBlocks.Core.Event;

namespace Passenger.Identity.Consumers.RegisterNewUser.Events.Domain.V1;

public record PassengerCreatedDomainEvent(long Id, string Name, string PassportNumber, bool IsDeleted = false) : IDomainEvent;
