using Griffin.Core.Exception;

namespace Flight.Airports.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException()
        : base("Name cannot be empty or whitespace.") { }
}
