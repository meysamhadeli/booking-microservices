namespace Passenger.Passengers.Exceptions;

using Griffin.Core.Exception;

public class InvalidAgeException : BadRequestException
{
    public InvalidAgeException() : base("Age Cannot be null or negative")
    {
    }
}