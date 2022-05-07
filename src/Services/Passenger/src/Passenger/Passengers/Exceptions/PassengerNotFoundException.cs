using BuildingBlocks.Exception;

namespace Passenger.Passengers.Exceptions;

public class PassengerNotFoundException: NotFoundException
{
    public PassengerNotFoundException(string code = default) : base("Passenger not found!")
    {
    }
}
