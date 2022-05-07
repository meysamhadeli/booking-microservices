using BuildingBlocks.Exception;

namespace Flight.Seats.Exceptions;

public class SeatAlreadyChosenException : ConflictException
{
    public SeatAlreadyChosenException(string code = default) : base("Seat already chosen!", code)
    {
    }
}
