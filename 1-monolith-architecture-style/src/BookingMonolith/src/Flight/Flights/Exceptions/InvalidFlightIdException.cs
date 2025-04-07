using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidFlightIdException : BadRequestException
{
    public InvalidFlightIdException(Guid flightId)
        : base($"flightId: '{flightId}' is invalid.")
    {
    }
}
