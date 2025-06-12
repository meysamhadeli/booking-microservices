using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidFlightNumberException : DomainException
{
    public InvalidFlightNumberException(string flightNumber)
        : base($"Flight Number: '{flightNumber}' is invalid.")
    {
    }
}
