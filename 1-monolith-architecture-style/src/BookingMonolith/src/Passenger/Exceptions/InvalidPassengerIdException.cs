using BuildingBlocks.Exception;

namespace BookingMonolith.Passenger.Exceptions;

public class InvalidPassengerIdException : BadRequestException
{
    public InvalidPassengerIdException(Guid passengerId)
        : base($"PassengerId: '{passengerId}' is invalid.")
    {
    }
}
