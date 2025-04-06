namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidArriveDateException : BadRequestException
{
    public InvalidArriveDateException(DateTime arriveDate)
        : base($"Arrive Date: '{arriveDate}' is invalid.")
    {
    }
}
