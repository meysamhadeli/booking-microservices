namespace Passenger.Passengers.Exceptions;

using Woo.Core.Exception;


public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base("Name cannot be empty or whitespace.")
    {
    }
}