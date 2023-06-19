namespace Flight.Airports.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidAirportIdException : BadRequestException
{
    public InvalidAirportIdException(Guid airportId)
        : base($"airportId: '{airportId}' is invalid.")
    {
    }
}
