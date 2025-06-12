using SmartCharging.Infrastructure.Exceptions;

namespace BookingMonolith.Passenger.Exceptions;

public class InvalidPassportNumberException : DomainException
{
    public InvalidPassportNumberException() : base("Passport number cannot be empty or whitespace.")
    {
    }
}
