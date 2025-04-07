using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Aircrafts.Exceptions;

public class InvalidModelException : BadRequestException
{
    public InvalidModelException() : base("Model cannot be empty or whitespace.")
    {
    }
}
