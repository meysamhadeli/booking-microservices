namespace Passenger.Passengers.Exceptions;
using BuildingBlocks.Exception;

public class InvalidAgeException : BadRequestException
{
    public InvalidAgeException() : base("Age Cannot be null or negative")
    {
    }
}
