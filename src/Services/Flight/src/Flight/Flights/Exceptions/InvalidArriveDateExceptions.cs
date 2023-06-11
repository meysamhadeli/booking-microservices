namespace Flight.Flights.Exceptions;
using System;
using BuildingBlocks.Exception;

public class InvalidArriveDateExceptions : BadRequestException
{
    public InvalidArriveDateExceptions(DateTime arriveDate)
        : base($"Arrive Date: '{arriveDate}' is invalid.")
    {
    }
}
