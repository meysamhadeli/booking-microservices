using SmartCharging.Infrastructure.Exceptions;

namespace Flight.Seats.Exceptions;

public class InvalidSeatNumberException : DomainException
{
    public InvalidSeatNumberException() : base("SeatNumber Cannot be null or negative")
    {
    }
}
