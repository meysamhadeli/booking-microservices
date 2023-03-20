namespace Flight.Seats.Exceptions;

using BuildingBlocks.Exception;

public class SeatNumberIncorrectException : BadRequestException
{
    public SeatNumberIncorrectException() : base("Seat number is incorrect!")
    {
    }
}
