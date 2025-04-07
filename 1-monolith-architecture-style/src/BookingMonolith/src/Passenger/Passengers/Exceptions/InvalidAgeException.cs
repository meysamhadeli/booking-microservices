using BuildingBlocks.Exception;

namespace BookingMonolith.Passenger.Passengers.Exceptions;

public class InvalidAgeException : BadRequestException
{
    public InvalidAgeException() : base("Age Cannot be null or negative")
    {
    }
}
