using SmartCharging.Infrastructure.Exceptions;

namespace Passenger.Exceptions;

using System;

public class InvalidPassengerIdException : DomainException
{
    public InvalidPassengerIdException(Guid passengerId)
        : base($"PassengerId: '{passengerId}' is invalid.")
    {
    }
}