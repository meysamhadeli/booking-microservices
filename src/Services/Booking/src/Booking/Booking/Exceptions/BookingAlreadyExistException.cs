using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class BookingAlreadyExistException : ConflictException
{
    public BookingAlreadyExistException(string code = default) : base("Booking already exist!", code)
    {
    }
}
