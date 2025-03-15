namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidFlightIdException : BadRequestException
{
    public InvalidFlightIdException(Guid flightId)
        : base($"flightId: '{flightId}' is invalid.")
    {
    }
}
