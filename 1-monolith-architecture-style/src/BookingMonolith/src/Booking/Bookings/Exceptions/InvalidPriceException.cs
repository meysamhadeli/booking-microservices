using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidPriceException : BadRequestException
{
    public InvalidPriceException(decimal price)
        : base($"Price: '{price}' must be grater than or equal 0.")
    {
    }
}

