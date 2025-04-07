using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidArriveAirportIdException : BadRequestException
{
    public InvalidArriveAirportIdException(Guid arriveAirportId)
        : base($"arriveAirportId: '{arriveAirportId}' is invalid.")
    {
    }
}
