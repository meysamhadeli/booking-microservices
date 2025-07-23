using SmartCharging.Infrastructure.Exceptions;

namespace Flight.Airports.Exceptions;

public class InvalidAddressException : DomainException
{
    public InvalidAddressException() : base("Address cannot be empty or whitespace.")
    {
    }
}