namespace Passenger.Passengers.Exceptions;
using BuildingBlocks.Exception;


public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base("Name cannot be empty or whitespace.")
    {
    }
}
