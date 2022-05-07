using BuildingBlocks.Exception;

namespace Flight.Seats.Exceptions;

public class SeatNumberIncorrectException : BadRequestException
{
    public SeatNumberIncorrectException() : base("Seat number is incorrect!")
    {
    }
}
