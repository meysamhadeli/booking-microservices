namespace Flight.Flights.Exceptions;
using BuildingBlocks.Exception;

public class InvalidDurationException : BadRequestException
{
    public InvalidDurationException()
        : base("Duration cannot be negative.")
    {
    }
}
