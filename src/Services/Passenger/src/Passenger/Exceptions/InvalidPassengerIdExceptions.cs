namespace Passenger.Exceptions;
using System;

using BuildingBlocks.Exception;

public class InvalidPassengerIdExceptions : BadRequestException
{
    public InvalidPassengerIdExceptions(Guid passengerId)
        : base($"PassengerId: '{passengerId}' is invalid.")
    {
    }
}
