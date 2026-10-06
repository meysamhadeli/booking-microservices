namespace Passenger.Passengers.Exceptions;

using Griffin.Core.Exception;

public class PassengerNotFoundException : NotFoundException
{
    public PassengerNotFoundException(string code = default) : base("Passenger not found!")
    {
    }
}