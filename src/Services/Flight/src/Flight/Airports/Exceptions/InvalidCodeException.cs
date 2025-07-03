using SmartCharging.Infrastructure.Exceptions;

namespace Flight.Airports.Exceptions;

public class InvalidCodeException : DomainException
{
    public InvalidCodeException() : base("Code cannot be empty or whitespace.")
    {
    }
}
