using BuildingBlocks.Exception;

namespace Flight.Seats.Exceptions;

public class AllSeatsFullException : BadRequestException
{
    public AllSeatsFullException() : base("All seats are full!")
    {
    }
}
