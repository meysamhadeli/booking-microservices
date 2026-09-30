using System.Net;
using Woo.Core.Exception;

namespace Flight.Flights.Exceptions;

public class FlightAlreadyExistException : AppException
{
    public FlightAlreadyExistException(int? code = default) : base("Flight already exist!", HttpStatusCode.Conflict, code)
    {
    }
}