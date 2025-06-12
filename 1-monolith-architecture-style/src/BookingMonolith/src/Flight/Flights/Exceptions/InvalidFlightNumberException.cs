using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidFlightNumberException : DomainException
{
    public InvalidFlightNumberException(string flightNumber)
        : base($"Flight Number: '{flightNumber}' is invalid.")
    {
    }
}
