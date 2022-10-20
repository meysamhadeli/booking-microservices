using BuildingBlocks.Exception;

namespace Flight.Seats.Features.CreateSeat.Exceptions;

public class SeatAlreadyExistException : ConflictException
{
    public SeatAlreadyExistException(int? code = default) : base("Seat already exist!", code)
    {
    }
}
