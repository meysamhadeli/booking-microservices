namespace Passenger.Exceptions;
using System;

using BuildingBlocks.Exception;

public class InvalidPassengerIdException : BadRequestException
{
    public InvalidPassengerIdException(Guid passengerId)
        : base($"PassengerId: '{passengerId}' is invalid.")
    {
    }
}
