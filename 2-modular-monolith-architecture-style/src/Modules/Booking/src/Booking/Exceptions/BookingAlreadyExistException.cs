using System.Net;
using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class BookingAlreadyExistException : AppException
{
    public BookingAlreadyExistException(int? code = default) : base("Booking already exist!", HttpStatusCode.Conflict, code)
    {
    }
}
