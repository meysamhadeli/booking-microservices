using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidPriceException : DomainException
{
    public InvalidPriceException()
        : base($"Price Cannot be negative.")
    {
    }
}
