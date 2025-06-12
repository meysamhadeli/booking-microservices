using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidFlightIdException : DomainException
{
    public InvalidFlightIdException(Guid flightId)
        : base($"flightId: '{flightId}' is invalid.")
    {
    }
}
