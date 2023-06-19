namespace Booking.Booking.Exceptions;

using BuildingBlocks.Exception;

public class SeatNumberException : BadRequestException
{
    public SeatNumberException(string seatNumber)
        : base($"Seat Number: '{seatNumber}' is invalid.")
    {
    }
}

