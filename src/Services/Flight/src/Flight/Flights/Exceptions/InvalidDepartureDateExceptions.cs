namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidDepartureDateExceptions : BadRequestException
{
    public InvalidDepartureDateExceptions(DateTime departureDate)
        : base($"Departure Date: '{departureDate}' is invalid.")
    {
    }
}
