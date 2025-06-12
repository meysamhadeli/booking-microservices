using System.Net;
using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class BookingAlreadyExistException : AppException
{
    public BookingAlreadyExistException(int? code = default) : base("Booking already exist!", HttpStatusCode.Conflict, code)
    {
    }
}
