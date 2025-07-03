using SmartCharging.Infrastructure.Exceptions;

namespace Flight.Flights.Exceptions;

public class InvalidDepartureDateException : DomainException
{
    public InvalidDepartureDateException(DateTime departureDate)
        : base($"Departure Date: '{departureDate}' is invalid.")
    {
    }
}
