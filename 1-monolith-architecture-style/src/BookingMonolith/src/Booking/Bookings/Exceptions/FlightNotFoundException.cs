using System.Net;
using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class FlightNotFoundException : AppException
{
    public FlightNotFoundException() : base("Flight doesn't exist!", HttpStatusCode.NotFound)
    {
    }
}
