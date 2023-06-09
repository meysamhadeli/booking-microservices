namespace Flight.Airports.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidAirportIdExceptions : BadRequestException
{
    public InvalidAirportIdExceptions(Guid airportId)
        : base($"airportId: '{airportId}' is invalid.")
    {
    }
}
