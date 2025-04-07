using BuildingBlocks.Core.Event;

namespace BookingMonolith.Passenger.Identity.Consumers.RegisteringNewUser.V1;

public record PassengerCreatedDomainEvent(Guid Id, string Name, string PassportNumber, bool IsDeleted = false) : IDomainEvent;
