using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class FlightNotFoundException : NotFoundException
{
    public FlightNotFoundException() : base("Flight doesn't exist!")
    {
    }
}
