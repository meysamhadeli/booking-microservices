namespace Passenger.Passengers.Exceptions;

using Woo.Core.Exception;

public class PassengerNotFoundException : NotFoundException
{
    public PassengerNotFoundException(string code = default) : base("Passenger not found!")
    {
    }
}