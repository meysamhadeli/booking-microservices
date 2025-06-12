using System.Net;
using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Flights.Exceptions;

public class FlightNotFountException : AppException
{
    public FlightNotFountException() : base("Flight not found!", HttpStatusCode.NotFound)
    {
    }
}
