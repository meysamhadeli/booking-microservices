namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class InvalidDepartureAirportIdException : BadRequestException
{
    public InvalidDepartureAirportIdException(Guid departureAirportId)
        : base($"departureAirportId: '{departureAirportId}' is invalid.")
    {
    }
}
