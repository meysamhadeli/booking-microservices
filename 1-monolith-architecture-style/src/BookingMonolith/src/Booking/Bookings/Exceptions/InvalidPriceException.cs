using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidPriceException : DomainException
{
    public InvalidPriceException(decimal price)
        : base($"Price: '{price}' must be grater than or equal 0.")
    {
    }
}

