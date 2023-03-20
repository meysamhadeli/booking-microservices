namespace Passenger.Passengers.Exceptions;

using BuildingBlocks.Exception;

public class PassengerNotFoundException: NotFoundException
{
    public PassengerNotFoundException(string code = default) : base("Passenger not found!")
    {
    }
}
