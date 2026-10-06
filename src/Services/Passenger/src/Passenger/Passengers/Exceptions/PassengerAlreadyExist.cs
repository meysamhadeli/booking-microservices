namespace Passenger.Passengers.Exceptions;

using Griffin.Core.Exception;

public class PassengerNotExist : BadRequestException
{
    public PassengerNotExist(string code = default) : base("Please register before!")
    {
    }
}