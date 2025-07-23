using SmartCharging.Infrastructure.Exceptions;

namespace Flight.Aircrafts.Exceptions;

public class InvalidModelException : DomainException
{
    public InvalidModelException() : base("Model cannot be empty or whitespace.")
    {
    }
}