namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class InvalidArriveAirportIdException : BadRequestException
{
    public InvalidArriveAirportIdException(Guid arriveAirportId)
        : base($"arriveAirportId: '{arriveAirportId}' is invalid.")
    {
    }
}
