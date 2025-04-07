using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidDurationException : BadRequestException
{
    public InvalidDurationException()
        : base("Duration cannot be negative.")
    {
    }
}
