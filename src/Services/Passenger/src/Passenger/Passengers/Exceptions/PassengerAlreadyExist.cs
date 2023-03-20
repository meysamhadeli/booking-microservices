namespace Passenger.Passengers.Exceptions;

using BuildingBlocks.Exception;

public class PassengerNotExist : BadRequestException
{
    public PassengerNotExist(string code = default) : base("Please register before!")
    {
    }
}
