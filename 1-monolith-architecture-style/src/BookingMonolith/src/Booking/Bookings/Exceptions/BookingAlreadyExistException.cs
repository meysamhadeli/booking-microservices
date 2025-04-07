using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class BookingAlreadyExistException : ConflictException
{
    public BookingAlreadyExistException(int? code = default) : base("Booking already exist!", code)
    {
    }
}
