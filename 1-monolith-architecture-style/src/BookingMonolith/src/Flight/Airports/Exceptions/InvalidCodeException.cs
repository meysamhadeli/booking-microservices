using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class InvalidCodeException : BadRequestException
{
    public InvalidCodeException() : base("Code cannot be empty or whitespace.")
    {
    }
}
