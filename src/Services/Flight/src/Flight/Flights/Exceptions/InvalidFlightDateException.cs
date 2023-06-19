namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidFlightDateException : BadRequestException
{
    public InvalidFlightDateException(DateTime flightDate)
        : base($"Flight Date: '{flightDate}' is invalid.")
    {
    }
}
