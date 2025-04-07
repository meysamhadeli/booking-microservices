using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class InvalidAddressException : BadRequestException
{
    public InvalidAddressException() : base("Address cannot be empty or whitespace.")
    {
    }
}
