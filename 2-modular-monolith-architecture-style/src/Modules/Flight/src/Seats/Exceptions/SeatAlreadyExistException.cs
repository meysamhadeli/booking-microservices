namespace Flight.Seats.Exceptions;

using BuildingBlocks.Exception;

public class SeatAlreadyExistException : ConflictException
{
    public SeatAlreadyExistException(int? code = default) : base("Seat already exist!", code)
    {
    }
}
