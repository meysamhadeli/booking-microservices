using BuildingBlocks.Exception;

namespace Flight.Seats.Exceptions;

public class SeatAlreadyExistException : ConflictException
{
    public SeatAlreadyExistException(string code = default) : base("Seat already exist!", code)
    {
    }
}
