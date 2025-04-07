using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Seats.Exceptions;

public class SeatNumberIncorrectException : BadRequestException
{
    public SeatNumberIncorrectException() : base("Seat number is incorrect!")
    {
    }
}
