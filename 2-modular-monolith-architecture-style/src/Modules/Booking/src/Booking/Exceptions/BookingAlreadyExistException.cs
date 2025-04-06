namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class BookingAlreadyExistException : ConflictException
{
    public BookingAlreadyExistException(int? code = default) : base("Booking already exist!", code)
    {
    }
}
