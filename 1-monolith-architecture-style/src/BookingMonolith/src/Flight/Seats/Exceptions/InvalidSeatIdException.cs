using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Seats.Exceptions;

public class InvalidSeatIdException : BadRequestException
{
    public InvalidSeatIdException(Guid seatId)
        : base($"seatId: '{seatId}' is invalid.")
    {
    }
}
