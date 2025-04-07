using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class InvalidFlightNumberException : BadRequestException
{
    public InvalidFlightNumberException(string flightNumber)
        : base($"Flight Number: '{flightNumber}' is invalid.")
    {
    }
}
