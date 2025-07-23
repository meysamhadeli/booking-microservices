using System.Net;
using BuildingBlocks.Exception;

namespace Flight.Flights.Exceptions;

public class FlightAlreadyExistException : AppException
{
    public FlightAlreadyExistException(int? code = default) : base("Flight already exist!", HttpStatusCode.Conflict, code)
    {
    }
}