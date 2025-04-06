namespace Flight.Airports.Exceptions;
using BuildingBlocks.Exception;


public class InvalidCodeException : BadRequestException
{
    public InvalidCodeException() : base("Code cannot be empty or whitespace.")
    {
    }
}
