using BuildingBlocks.Exception;

namespace Flight.Seats.Features.ReserveSeat.Exceptions;

public class SeatNumberIncorrectException : BadRequestException
{
    public SeatNumberIncorrectException() : base("Seat number is incorrect!")
    {
    }
}
