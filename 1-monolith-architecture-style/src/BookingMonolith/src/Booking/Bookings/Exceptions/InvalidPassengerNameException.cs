using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidPassengerNameException : DomainException
{
    public InvalidPassengerNameException(string passengerName)
        : base($"Passenger Name: '{passengerName}' is invalid.")
    {
    }
}
