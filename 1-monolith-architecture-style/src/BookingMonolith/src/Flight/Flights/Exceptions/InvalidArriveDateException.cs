using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class InvalidArriveDateException : BadRequestException
{
    public InvalidArriveDateException(DateTime arriveDate)
        : base($"Arrive Date: '{arriveDate}' is invalid.")
    {
    }
}
