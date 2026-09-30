using Woo.Core.Exception;

namespace Flight.Flights.Exceptions;

public class InvalidFlightNumberException : DomainException
{
    public InvalidFlightNumberException(string flightNumber)
        : base($"Flight Number: '{flightNumber}' is invalid.") { }
}
