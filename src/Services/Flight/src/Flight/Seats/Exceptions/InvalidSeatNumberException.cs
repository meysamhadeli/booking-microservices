namespace Flight.Seats.Exceptions;
using BuildingBlocks.Exception;

public class InvalidSeatNumberException : BadRequestException
{
    public InvalidSeatNumberException() : base("SeatNumber Cannot be null or negative")
    {
    }
}
