using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Seats.Exceptions;

public class InvalidSeatNumberException : BadRequestException
{
    public InvalidSeatNumberException() : base("SeatNumber Cannot be null or negative")
    {
    }
}
