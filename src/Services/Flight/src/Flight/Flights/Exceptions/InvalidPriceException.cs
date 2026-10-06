using Griffin.Core.Exception;

namespace Flight.Flights.Exceptions;

public class InvalidPriceException : DomainException
{
    public InvalidPriceException()
        : base($"Price Cannot be negative.") { }
}
