using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Seats.Exceptions;

public class AllSeatsFullException : BadRequestException
{
    public AllSeatsFullException() : base("All seats are full!")
    {
    }
}
