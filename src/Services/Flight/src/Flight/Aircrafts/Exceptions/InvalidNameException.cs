using Griffin.Core.Exception;

namespace Flight.Aircrafts.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException()
        : base("Name cannot be empty or whitespace.") { }
}
