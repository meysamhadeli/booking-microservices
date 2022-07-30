using BuildingBlocks.Core.Event;
using Passenger.Passengers.Models;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Reads;

public class CompleteRegisterPassengerMongoCommand : InternalCommand
{
    public CompleteRegisterPassengerMongoCommand(long id, string passportNumber, string name,
        PassengerType passengerType, int age, bool isDeleted)
    {
        Id = id;
        PassportNumber = passportNumber;
        Name = name;
        PassengerType = passengerType;
        Age = age;
        IsDeleted = isDeleted;
    }

    public string PassportNumber { get; }
    public string Name { get; }
    public PassengerType PassengerType { get; }
    public int Age { get; }
    public bool IsDeleted { get; }
}
