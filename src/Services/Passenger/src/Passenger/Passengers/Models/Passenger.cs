using BuildingBlocks.Core.Model;

namespace Passenger.Passengers.Models;

using Features.CompletingRegisterPassenger.V1;
using Identity.Consumers.RegisteringNewUser.V1;

public record Passenger : Aggregate<Guid>
{
    public Passenger CompleteRegistrationPassenger(Guid id, string name, string passportNumber, Enums.PassengerType passengerType, int age, bool isDeleted = false)
    {
        var passenger = new Passenger
        {
            Name = name,
            PassportNumber = passportNumber,
            PassengerType = passengerType,
            Age = age,
            Id = id,
            IsDeleted = isDeleted
        };

        var @event = new PassengerRegistrationCompletedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber,
            passenger.PassengerType, passenger.Age, passenger.IsDeleted);

        passenger.AddDomainEvent(@event);

        return passenger;
    }


    public static Passenger Create(Guid id, string name, string passportNumber, bool isDeleted = false)
    {
        var passenger = new Passenger { Id = id, Name = name, PassportNumber = passportNumber, IsDeleted = isDeleted };

        var @event = new PassengerCreatedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber, passenger.IsDeleted);

        passenger.AddDomainEvent(@event);

        return passenger;
    }


    public string PassportNumber { get; private set; }
    public string Name { get; private set; }
    public Enums.PassengerType PassengerType { get; private set; }
    public int Age { get; private set; }
}
