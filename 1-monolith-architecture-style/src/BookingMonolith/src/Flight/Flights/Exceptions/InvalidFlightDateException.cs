using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidFlightDateException : BadRequestException
{
    public InvalidFlightDateException(DateTime flightDate)
        : base($"Flight Date: '{flightDate}' is invalid.")
    {
    }
}
