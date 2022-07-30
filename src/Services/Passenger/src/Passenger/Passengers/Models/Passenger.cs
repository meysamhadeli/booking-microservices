using BuildingBlocks.Core.Model;
using Passenger.Passengers.Events.Domain;

namespace Passenger.Passengers.Models;

public record Passenger : Aggregate<long>
{
    public Passenger CompleteRegistrationPassenger(long id, string name, string passportNumber, PassengerType passengerType, int age, bool isDeleted = false)
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


    public static Passenger Create(long id, string name, string passportNumber, bool isDeleted = false)
    {
        var passenger = new Passenger { Id = id, Name = name, PassportNumber = passportNumber, IsDeleted = isDeleted };

        var @event = new PassengerCreatedDomainEvent(passenger.Id, passenger.Name, passenger.PassportNumber, passenger.IsDeleted);

        passenger.AddDomainEvent(@event);

        return passenger;
    }


    public string PassportNumber { get; private set; }
    public string Name { get; private set; }
    public PassengerType PassengerType { get; private set; }
    public int Age { get; private set; }
}
