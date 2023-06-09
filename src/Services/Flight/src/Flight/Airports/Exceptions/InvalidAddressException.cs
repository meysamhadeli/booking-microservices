namespace Flight.Airports.Exceptions;
using BuildingBlocks.Exception;

public class InvalidAddressException : BadRequestException
{
    public InvalidAddressException() : base("Address cannot be empty or whitespace.")
    {
    }
}
