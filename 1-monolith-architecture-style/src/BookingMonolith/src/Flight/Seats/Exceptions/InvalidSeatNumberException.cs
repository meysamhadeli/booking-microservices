using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Flight.Seats.Exceptions;

public class InvalidSeatNumberException : DomainException
{
    public InvalidSeatNumberException() : base("SeatNumber Cannot be null or negative")
    {
    }
}
