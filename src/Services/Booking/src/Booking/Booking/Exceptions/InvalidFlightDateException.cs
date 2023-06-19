namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class InvalidFlightDateException : BadRequestException
{
    public InvalidFlightDateException(DateTime flightDate)
        : base($"Flight Date: '{flightDate}' is invalid.")
    {
    }
}
