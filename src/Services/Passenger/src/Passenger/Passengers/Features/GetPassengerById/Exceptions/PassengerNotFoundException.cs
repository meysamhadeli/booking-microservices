using BuildingBlocks.Exception;

namespace Passenger.Passengers.Features.GetPassengerById.Exceptions;

public class PassengerNotFoundException: NotFoundException
{
    public PassengerNotFoundException(string code = default) : base("Passenger not found!")
    {
    }
}
