namespace Passenger.Passengers.Features.CompletingRegisterPassenger.V1;

using BuildingBlocks.Core.Event;

public record PassengerRegistrationCompletedDomainEvent(long Id, string Name, string PassportNumber,
    Enums.PassengerType PassengerType, int Age, bool IsDeleted = false) : IDomainEvent;
