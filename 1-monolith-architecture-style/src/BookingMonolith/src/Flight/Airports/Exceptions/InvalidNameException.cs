using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base("Name cannot be empty or whitespace.")
    {
    }
}
