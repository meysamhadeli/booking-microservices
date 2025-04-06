namespace Flight.Flights.Exceptions;
using BuildingBlocks.Exception;


public class InvalidPriceException : BadRequestException
{
    public InvalidPriceException()
        : base($"Price Cannot be negative.")
    {
    }
}
