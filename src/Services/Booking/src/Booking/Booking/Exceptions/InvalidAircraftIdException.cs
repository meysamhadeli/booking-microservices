using Griffin.Core.Exception;

namespace Booking.Booking.Exceptions;

public class InvalidAircraftIdException : DomainException
{
    public InvalidAircraftIdException(Guid aircraftId)
        : base($"aircraftId: '{aircraftId}' is invalid.") { }
}
