namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;
public class InvalidFlightNumberException : BadRequestException
{
    public InvalidFlightNumberException(string flightNumber)
        : base($"Flight Number: '{flightNumber}' is invalid.")
    {
    }
}
