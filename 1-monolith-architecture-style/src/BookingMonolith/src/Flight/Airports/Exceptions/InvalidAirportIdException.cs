using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class InvalidAirportIdException : BadRequestException
{
    public InvalidAirportIdException(Guid airportId)
        : base($"airportId: '{airportId}' is invalid.")
    {
    }
}
