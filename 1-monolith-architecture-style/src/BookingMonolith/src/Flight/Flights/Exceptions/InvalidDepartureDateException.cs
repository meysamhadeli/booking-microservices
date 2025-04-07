using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidDepartureDateException : BadRequestException
{
    public InvalidDepartureDateException(DateTime departureDate)
        : base($"Departure Date: '{departureDate}' is invalid.")
    {
    }
}
