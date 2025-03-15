namespace Flight.Aircrafts.Exceptions;
using System;
using BuildingBlocks.Exception;


public class InvalidAircraftIdException : BadRequestException
{
    public InvalidAircraftIdException(Guid aircraftId)
        : base($"AircraftId: '{aircraftId}' is invalid.")
    {
    }
}
