namespace Passenger.Passengers.Exceptions;

using Woo.Core.Exception;

public class InvalidAgeException : BadRequestException
{
    public InvalidAgeException() : base("Age Cannot be null or negative")
    {
    }
}