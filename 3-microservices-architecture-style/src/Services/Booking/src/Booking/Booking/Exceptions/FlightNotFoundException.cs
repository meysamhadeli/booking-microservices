namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class FlightNotFoundException : NotFoundException
{
    public FlightNotFoundException() : base("Flight doesn't exist!")
    {
    }
}
