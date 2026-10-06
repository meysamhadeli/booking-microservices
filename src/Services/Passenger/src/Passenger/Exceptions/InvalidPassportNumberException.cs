using Griffin.Core.Exception;

namespace Passenger.Exceptions;

public class InvalidPassportNumberException : DomainException
{
    public InvalidPassportNumberException()
        : base("Passport number cannot be empty or whitespace.") { }
}
