using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class InvalidAirportIdException : DomainException
{
    public InvalidAirportIdException(Guid airportId)
        : base($"airportId: '{airportId}' is invalid.")
    {
    }
}
