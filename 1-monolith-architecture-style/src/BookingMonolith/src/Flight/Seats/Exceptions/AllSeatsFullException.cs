using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Seats.Exceptions;

public class AllSeatsFullException : AppException
{
    public AllSeatsFullException() : base("All seats are full!")
    {
    }
}
