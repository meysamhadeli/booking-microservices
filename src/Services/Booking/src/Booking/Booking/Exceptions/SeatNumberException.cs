using SmartCharging.Infrastructure.Exceptions;

namespace Booking.Booking.Exceptions;

public class SeatNumberException : DomainException
{
    public SeatNumberException(string seatNumber)
        : base($"Seat Number: '{seatNumber}' is invalid.")
    {
    }
}