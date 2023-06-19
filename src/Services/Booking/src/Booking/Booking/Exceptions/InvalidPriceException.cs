namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class InvalidPriceException : BadRequestException
{
    public InvalidPriceException(decimal price)
        : base($"Price: '{price}' must be grater than or equal 0.")
    {
    }
}

