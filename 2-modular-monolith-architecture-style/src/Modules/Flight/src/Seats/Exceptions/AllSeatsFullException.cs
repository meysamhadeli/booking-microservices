namespace Flight.Seats.Exceptions;

using BuildingBlocks.Exception;

public class AllSeatsFullException : BadRequestException
{
    public AllSeatsFullException() : base("All seats are full!")
    {
    }
}
