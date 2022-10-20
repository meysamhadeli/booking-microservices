using BuildingBlocks.Core.Event;

namespace Passenger.Passengers.Features.CompleteRegisterPassenger.Commands.V1.Reads;

public class CompleteRegisterPassengerMongoCommand : InternalCommand
{
    public CompleteRegisterPassengerMongoCommand(long id, string passportNumber, string name,
        Enums.PassengerType passengerType, int age, bool isDeleted)
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
    public Enums.PassengerType PassengerType { get; }
    public int Age { get; }
    public bool IsDeleted { get; }
}
