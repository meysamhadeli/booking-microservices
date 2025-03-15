namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidDepartureDateException : BadRequestException
{
    public InvalidDepartureDateException(DateTime departureDate)
        : base($"Departure Date: '{departureDate}' is invalid.")
    {
    }
}
