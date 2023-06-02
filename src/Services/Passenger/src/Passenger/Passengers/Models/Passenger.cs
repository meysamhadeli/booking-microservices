using BuildingBlocks.Core.Model;

namespace Passenger.Passengers.Models;

using Features.CompletingRegisterPassenger.V1;
using global::Passenger.Passengers.Models.ValueObjects;
using global::Passenger.Passengers.ValueObjects;
using Identity.Consumers.RegisteringNewUser.V1;

public record Passenger : Aggregate<PassengerId>
{
    public PassportNumber PassportNumber { get; private set; } = default!;
    public Name Name { get; private set; } = default!;
    public Enums.PassengerType PassengerType { get; private set; }
    public Age? Age { get; private set; }

    public Passenger CompleteRegistrationPassenger(PassengerId id, Name name, PassportNumber passportNumber, Enums.PassengerType passengerType, Age age, bool isDeleted = false)
    {
        var passenger = new Passenger
        {
            Id = id,
            Name = name,
            PassportNumber = passportNumber,
            PassengerType = passengerType,
            Age = age,
            IsDeleted = isDeleted
        };

        var @event = new PassengerRegistrationCompletedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber,
            passenger.PassengerType, passenger.Age.Value, passenger.IsDeleted);

        passenger.AddDomainEvent(@event);

        return passenger;
    }


    public static Passenger Create(PassengerId id, Name name, PassportNumber passportNumber, bool isDeleted = false)
    {
        var passenger = new Passenger { Id = id, Name = name, PassportNumber = passportNumber, IsDeleted = isDeleted };

        var @event = new PassengerCreatedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber, passenger.IsDeleted);

        passenger.AddDomainEvent(@event);

        return passenger;
    }
}
