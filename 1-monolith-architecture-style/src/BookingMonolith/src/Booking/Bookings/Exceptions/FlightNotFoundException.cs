using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class FlightNotFoundException : NotFoundException
{
    public FlightNotFoundException() : base("Flight doesn't exist!")
    {
    }
}
