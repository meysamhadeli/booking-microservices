using BuildingBlocks.Exception;

namespace BookingMonolith.Booking.Bookings.Exceptions;

public class SeatNumberException : BadRequestException
{
    public SeatNumberException(string seatNumber)
        : base($"Seat Number: '{seatNumber}' is invalid.")
    {
    }
}

