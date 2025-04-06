namespace Passenger.Passengers.Exceptions;
using BuildingBlocks.Exception;


public class InvalidPassportNumberException : BadRequestException
{
    public InvalidPassportNumberException() : base("Passport number cannot be empty or whitespace.")
    {
    }
}
