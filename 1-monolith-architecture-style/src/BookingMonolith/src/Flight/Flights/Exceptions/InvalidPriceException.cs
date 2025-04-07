using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidPriceException : BadRequestException
{
    public InvalidPriceException()
        : base($"Price Cannot be negative.")
    {
    }
}
