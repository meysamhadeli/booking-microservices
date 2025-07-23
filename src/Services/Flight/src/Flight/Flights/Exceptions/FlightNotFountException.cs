using System.Net;
using BuildingBlocks.Exception;

namespace Flight.Flights.Exceptions;

public class FlightNotFountException : AppException
{
    public FlightNotFountException() : base("Flight not found!", HttpStatusCode.NotFound)
    {
    }
}