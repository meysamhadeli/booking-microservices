using BuildingBlocks.Exception;

namespace Booking.Booking.Features.CreateBooking.Exceptions;

public class FlightNotFoundException : NotFoundException
{
    public FlightNotFoundException() : base("Flight doesn't exist!")
    {
    }
}
