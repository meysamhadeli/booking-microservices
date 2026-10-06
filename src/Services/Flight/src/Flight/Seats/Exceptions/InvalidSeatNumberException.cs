using Griffin.Core.Exception;

namespace Flight.Seats.Exceptions;

public class InvalidSeatNumberException : DomainException
{
    public InvalidSeatNumberException()
        : base("SeatNumber Cannot be null or negative") { }
}
