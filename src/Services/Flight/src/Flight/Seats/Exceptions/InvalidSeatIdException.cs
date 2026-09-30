using Woo.Core.Exception;

namespace Flight.Seats.Exceptions;

public class InvalidSeatIdException : DomainException
{
    public InvalidSeatIdException(Guid seatId)
        : base($"seatId: '{seatId}' is invalid.") { }
}
