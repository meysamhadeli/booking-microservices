using BuildingBlocks.Exception;

namespace Flight.Seats.Exceptions;

public class SeatAlreadyChosenException : ConflictException
{
    public SeatAlreadyChosenException(int? code = default) : base("Seat already chosen!", code)
    {
    }
}
