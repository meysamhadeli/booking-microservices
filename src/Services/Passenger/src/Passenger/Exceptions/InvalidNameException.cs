using Woo.Core.Exception;

namespace Passenger.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException()
        : base("Name cannot be empty or whitespace.") { }
}
