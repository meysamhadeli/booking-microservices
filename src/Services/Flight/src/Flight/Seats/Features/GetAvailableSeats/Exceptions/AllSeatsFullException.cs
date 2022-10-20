using BuildingBlocks.Exception;

namespace Flight.Seats.Features.GetAvailableSeats.Exceptions;

public class AllSeatsFullException : BadRequestException
{
    public AllSeatsFullException() : base("All seats are full!")
    {
    }
}
