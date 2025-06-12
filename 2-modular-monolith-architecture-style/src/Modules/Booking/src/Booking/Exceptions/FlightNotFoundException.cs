using System.Net;
using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class FlightNotFoundException : AppException
{
    public FlightNotFoundException() : base("Flight doesn't exist!", HttpStatusCode.NotFound)
    {
    }
}
