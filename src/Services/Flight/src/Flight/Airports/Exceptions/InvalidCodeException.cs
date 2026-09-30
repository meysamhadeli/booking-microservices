using Woo.Core.Exception;

namespace Flight.Airports.Exceptions;

public class InvalidCodeException : DomainException
{
    public InvalidCodeException()
        : base("Code cannot be empty or whitespace.") { }
}
