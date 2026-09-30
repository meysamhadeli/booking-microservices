using Woo.Core.Exception;

namespace Flight.Flights.Exceptions;

public class InvalidFlightDateException : DomainException
{
    public InvalidFlightDateException(DateTime flightDate)
        : base($"Flight Date: '{flightDate}' is invalid.") { }
}
