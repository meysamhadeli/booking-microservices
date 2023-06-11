namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidFlightDateExceptions : BadRequestException
{
    public InvalidFlightDateExceptions(DateTime flightDate)
        : base($"Flight Date: '{flightDate}' is invalid.")
    {
    }
}
