namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class InvalidAircraftIdException : BadRequestException
{
    public InvalidAircraftIdException(Guid aircraftId)
        : base($"aircraftId: '{aircraftId}' is invalid.")
    {
    }
}
