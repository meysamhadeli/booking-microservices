using BuildingBlocks.Exception;

namespace BookingMonolith.Passenger.Passengers.Exceptions;

public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base("Name cannot be empty or whitespace.")
    {
    }
}
