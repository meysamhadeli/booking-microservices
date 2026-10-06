using Griffin.Core.Exception;

namespace Booking.Booking.Exceptions;

public class InvalidPassengerNameException : DomainException
{
    public InvalidPassengerNameException(string passengerName)
        : base($"Passenger Name: '{passengerName}' is invalid.") { }
}
