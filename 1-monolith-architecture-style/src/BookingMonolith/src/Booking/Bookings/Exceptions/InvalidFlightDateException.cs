using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidFlightDateException : DomainException
{
    public InvalidFlightDateException(DateTime flightDate)
        : base($"Flight Date: '{flightDate}' is invalid.")
    {
    }
}
