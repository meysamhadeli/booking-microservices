using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class SeatNumberException : DomainException
{
    public SeatNumberException(string seatNumber)
        : base($"Seat Number: '{seatNumber}' is invalid.")
    {
    }
}

