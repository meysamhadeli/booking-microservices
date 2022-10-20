using BuildingBlocks.Core.Event;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Events.Domain.V1;

public record PassengerRegistrationCompletedDomainEvent(long Id, string Name, string PassportNumber,
    Enums.PassengerType PassengerType, int Age, bool IsDeleted = false) : IDomainEvent;
