using Woo.Core.Exception;

namespace Booking.Booking.Exceptions;

public class InvalidFlightDateException : DomainException
{
    public InvalidFlightDateException(DateTime flightDate)
        : base($"Flight Date: '{flightDate}' is invalid.") { }
}
