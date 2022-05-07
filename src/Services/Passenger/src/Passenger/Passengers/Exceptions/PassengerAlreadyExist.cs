using BuildingBlocks.Exception;

namespace Passenger.Passengers.Exceptions;

public class PassengerNotExist : BadRequestException
{
    public PassengerNotExist(string code = default) : base("Please register before!")
    {
    }
}
