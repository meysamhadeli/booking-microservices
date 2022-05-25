using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class BookingAlreadyExistException : ConflictException
{
    public BookingAlreadyExistException(int? code = default) : base("Booking already exist!", code)
    {
    }
}
