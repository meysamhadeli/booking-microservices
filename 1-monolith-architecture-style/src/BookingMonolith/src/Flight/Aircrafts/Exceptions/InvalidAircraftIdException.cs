using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Aircrafts.Exceptions;

public class InvalidAircraftIdException : BadRequestException
{
    public InvalidAircraftIdException(Guid aircraftId)
        : base($"AircraftId: '{aircraftId}' is invalid.")
    {
    }
}
