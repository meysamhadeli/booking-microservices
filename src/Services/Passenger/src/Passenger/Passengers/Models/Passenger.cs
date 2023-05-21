using BuildingBlocks.Core.Model;

namespace Passenger.Passengers.Models;

using Features.CompletingRegisterPassenger.V1;
using global::Passenger.Passengers.Models.ValueObjects;
using Identity.Consumers.RegisteringNewUser.V1;

public record Passenger : Aggregate<Guid>
{
    public Passenger CompleteRegistrationPassenger(Guid id, NameValue name, PassportNumberValue passportNumber, Enums.PassengerType passengerType, AgeValue age, bool isDeleted = false)
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


    public static Passenger Create(Guid id, NameValue name, PassportNumberValue passportNumber, bool isDeleted = false)
    {
        var passenger = new Passenger { Id = id, Name = name, PassportNumber = passportNumber, IsDeleted = isDeleted };

        var @event = new PassengerCreatedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber, passenger.IsDeleted);

        passenger.AddDomainEvent(@event);

        return passenger;
    }


    public PassportNumberValue PassportNumber { get; private set; }
    public NameValue Name { get; private set; }
    public Enums.PassengerType PassengerType { get; private set; }
    public AgeValue Age { get; private set; }
}
