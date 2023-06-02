namespace Flight.Aircrafts.Exceptions;
using System;
using BuildingBlocks.Exception;


public class InvalidAircraftIdExceptions : BadRequestException
{
    public InvalidAircraftIdExceptions(Guid aircraftId)
        : base($"AircraftId: '{aircraftId}' is invalid.")
    {
    }
}
