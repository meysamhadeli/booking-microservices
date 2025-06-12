using BuildingBlocks.Exception;

namespace Flight.Seats.Exceptions;

public class SeatNumberIncorrectException : AppException
{
    public SeatNumberIncorrectException() : base("Seat number is incorrect!")
    {
    }
}
