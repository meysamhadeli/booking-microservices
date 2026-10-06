using Griffin.Core.Exception;

namespace Flight.Flights.Exceptions;

public class InvalidDurationException : DomainException
{
    public InvalidDurationException()
        : base("Duration cannot be negative.") { }
}
