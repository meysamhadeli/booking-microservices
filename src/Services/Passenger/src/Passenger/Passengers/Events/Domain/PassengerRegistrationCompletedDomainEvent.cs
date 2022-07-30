using BuildingBlocks.Core.Event;
using Passenger.Passengers.Models;

namespace Passenger.Passengers.Events.Domain;

public record PassengerRegistrationCompletedDomainEvent(long Id, string Name, string PassportNumber,
    PassengerType PassengerType, int Age, bool IsDeleted = false) : IDomainEvent;
