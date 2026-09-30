namespace Passenger.Passengers.Exceptions;

using Woo.Core.Exception;


public class InvalidPassportNumberException : BadRequestException
{
    public InvalidPassportNumberException() : base("Passport number cannot be empty or whitespace.")
    {
    }
}