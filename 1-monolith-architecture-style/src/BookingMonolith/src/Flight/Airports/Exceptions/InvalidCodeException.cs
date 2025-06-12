using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class InvalidCodeException : DomainException
{
    public InvalidCodeException() : base("Code cannot be empty or whitespace.")
    {
    }
}
