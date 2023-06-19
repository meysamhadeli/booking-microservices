namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class InvalidPassengerNameException : BadRequestException
{
    public InvalidPassengerNameException(string passengerName)
        : base($"Passenger Name: '{passengerName}' is invalid.")
    {
    }
}
