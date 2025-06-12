using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Passenger.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException() : base("Name cannot be empty or whitespace.")
    {
    }
}
