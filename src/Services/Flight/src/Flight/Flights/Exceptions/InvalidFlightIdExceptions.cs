namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidFlightIdExceptions : BadRequestException
{
    public InvalidFlightIdExceptions(Guid flightId)
        : base($"flightId: '{flightId}' is invalid.")
    {
    }
}
